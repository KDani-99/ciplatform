using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Exceptions;
using CIPlatformWorker.Services;
using CIPlatform.Core.Hubs.Consumers;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using CIPlatform.Data.Extensions;
using CIPlatform.Data.JsonWebTokens;
using Microsoft.Extensions.Logging;

namespace CIPlatformWorker.WebSocket.Consumers
{
    public class QueueJobCommandConsumer : IConsumer<QueueJobCommand>
    {
        private readonly ILogger<QueueJobCommandConsumer> _logger;
        private readonly IWorkerClient _workerClient;
        private readonly IJobHandlerService _jobHandlerService;
        private int _stepIndex = 0;

        public QueueJobCommandConsumer(ILogger<QueueJobCommandConsumer> logger,
                                       IWorkerClient workerClient,
                                       IJobHandlerService jobHandlerService)
        {
            _jobHandlerService = jobHandlerService ??
                throw new ArgumentNullException(nameof(jobHandlerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _workerClient = workerClient ?? throw new ArgumentNullException(nameof(workerClient));
        }

        public async Task ConsumeAsync(QueueJobCommand queueJobCommand, CancellationToken cancellationToken = default)
        {
            var jobDetails = GetJobDetails(queueJobCommand);

            try
            {
                _logger.LogInformation(
                    $"Starting {queueJobCommand.JobConfiguration.Context.ToString()} job: {jobDetails.JobId} (Run: {jobDetails.RunId})...");
                await ProcessJobAsync(jobDetails, queueJobCommand.JobConfiguration, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Job was cancelled.");
                await _workerClient.SendStepResultAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = _stepIndex,
                    Token = queueJobCommand.Token
                });
            }
            catch (StepFailedException exception)
            {
                _logger.LogError($"Step '{exception.Name}' failed. Exit code was {exception.ExitCode}.");
                await _workerClient.SendStepResultAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = _stepIndex,
                    Token = queueJobCommand.Token
                });
            }
            catch (Exception exception)
            {
                _logger.LogError($"An unexpected error has occured. Error: {exception.Message}");
                await _workerClient.SendStepResultAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = _stepIndex,
                    Token = queueJobCommand.Token
                });
                // rest will be marked as skipped
            }
            finally
            {
                _logger.LogInformation($"Finished job: {jobDetails.JobId} (Run: {jobDetails.RunId}).");
            }
        }

        private async Task ProcessJobAsync(JobDetails jobDetails,
                                           JobConfiguration jobConfiguration,
                                           CancellationToken cancellationToken = default)
        {
            var beforeExecutionDateTime = DateTime.Now;

            await _jobHandlerService.PrepareEnvironmentAsync();

            for (var i = 0; i < jobConfiguration.Steps.Count; i++)
            {
                var channel = Channel.CreateUnbounded<string>();

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _ = _workerClient.StreamLogAsync(jobDetails.RunId, jobDetails.JobId, i,
                                                     channel.Reader); // do not await!

                    await ProcessStepAsync(channel, i, _jobHandlerService, jobConfiguration.Steps[i], jobDetails);
                }
                finally // try-finally to make sure the channel write gets completed
                {
                    channel.Writer.Complete();
                }

                _stepIndex++;
            }

            var afterExecutionDateTime = DateTime.Now;
            var totalTime = afterExecutionDateTime - beforeExecutionDateTime;

            _logger.LogInformation($"Job ran for {totalTime.TotalSeconds} second(s).");
        }

        private async Task ProcessStepAsync(ChannelWriter<string> channelWriter,
                                            int stepIndex,
                                            IJobHandlerService jobHandlerService,
                                            StepConfiguration step,
                                            JobDetails jobDetails)
        {
            _logger.LogInformation($"Executing step {step.Name}...");

            await _workerClient.SendStepResultAsync(new StepResultEvent
            {
                State = States.Running,
                StepIndex = stepIndex,
                Token = jobDetails.Token
            });

            await jobHandlerService.ExecuteStepAsync(channelWriter, step, stepIndex);

            _logger.LogInformation($"Successfully executed step {step.Name}.");
            await _workerClient.SendStepResultAsync(new StepResultEvent
            {
                State = States.Successful,
                StepIndex = stepIndex,
                Token = jobDetails.Token
            });
        }

        private static JobDetails GetJobDetails(QueueJobCommand queueJobCommand)
        {
            var decodedToken = queueJobCommand.Token.DecodeJwtToken();
            var runId = long.Parse(decodedToken.Claims
                                               .FirstOrDefault(
                                                   claim => claim.Type == CustomJwtRegisteredClaimNames.RunId)?.Value!);
            var jobId = long.Parse(decodedToken.Claims
                                               .FirstOrDefault(
                                                   claim => claim.Type == CustomJwtRegisteredClaimNames.JobId)?.Value!);

            return new JobDetails
            {
                Token = queueJobCommand.Token,
                Repository = queueJobCommand.Repository,
                RunId = runId,
                JobId = jobId
            };
        }
    }
}