using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatform.Core.SignalR.Consumers;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using CIPlatform.Data.Extensions;
using CIPlatform.Data.JsonWebTokens;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Exceptions;
using CIPlatformWorker.Factories;
using CIPlatformWorker.Factories.Job;
using CIPlatformWorker.Services;
using CIPlatformWorker.Services.Job;
using Microsoft.Extensions.Logging;

namespace CIPlatformWorker.SignalR.Consumers
{
    public class QueueJobCommandConsumer : IConsumer<QueueJobCommand>
    {
        private readonly ILogger<QueueJobCommandConsumer> _logger;
        private readonly IWorkerClient _workerClient;
        private readonly IJobHandlerServiceFactory _jobHandlerServiceFactory;
        private int _stepIndex;

        public QueueJobCommandConsumer(IJobHandlerServiceFactory jobHandlerServiceFactory,
                                       ILogger<QueueJobCommandConsumer> logger,
                                       IWorkerClient workerClient)
        {
            _jobHandlerServiceFactory = jobHandlerServiceFactory ??
                throw new ArgumentNullException(nameof(jobHandlerServiceFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _workerClient = workerClient ?? throw new ArgumentNullException(nameof(workerClient));
        }

        public async Task ConsumeAsync(QueueJobCommand queueJobCommand, CancellationToken cancellationToken = default)
        {
            var jobDetails = GetJobDetails(queueJobCommand);
            var beforeExecutionDateTime = DateTime.Now;

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
                _logger.LogError(exception, $"An unexpected error has occured. Error: {exception.Message}");
                await _workerClient.SendStepResultAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = _stepIndex,
                    Token = queueJobCommand.Token
                });
            }
            finally
            {
                _logger.LogInformation($"Finished job: {jobDetails.JobId} (Run: {jobDetails.RunId}).");

                var totalTime = DateTime.Now - beforeExecutionDateTime;
                _logger.LogInformation($"Job ran for {totalTime.TotalSeconds} second(s).");

                await _workerClient.FinishJobAsync();
            }
        }

        private async Task ProcessJobAsync(JobDetails jobDetails,
                                           JobConfiguration jobConfiguration,
                                           CancellationToken cancellationToken = default)
        {
            await using var jobHandlerService = _jobHandlerServiceFactory.Create(jobConfiguration, cancellationToken);
            await jobHandlerService.PrepareEnvironmentAsync();

            for (var i = 0; i < jobConfiguration.Steps.Count; i++)
            {
                var channel = Channel.CreateUnbounded<string>();

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Do not await!
                    _ = _workerClient.StreamLogAsync(jobDetails.RunId, jobDetails.JobId, i,
                        channel.Reader);

                    await ProcessStepAsync(channel, i, jobHandlerService, jobConfiguration.Steps[i], jobDetails);
                }
                finally
                {
                    // try-finally to make sure the channel write gets completed
                    channel.Writer.Complete();
                }

                _stepIndex++;
            }
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