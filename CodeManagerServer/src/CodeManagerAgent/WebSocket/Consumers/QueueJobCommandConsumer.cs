using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Common;
using CodeManager.Core.Hubs.Consumers;
using CodeManager.Data.Agent;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.JsonWebTokens;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Exceptions;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.WebSocket.Consumers
{
    public class QueueJobCommandConsumer : IConsumer<QueueJobCommand>
    {
        private readonly IAgentService _agentService;
        private readonly IJobHandlerServiceFactory _jobHandlerServiceFactory;
        private readonly ILogger<QueueJobCommandConsumer> _logger;
        private readonly IWorkerClient _workerClient;

        public QueueJobCommandConsumer(IAgentService agentService,
            IJobHandlerServiceFactory jobHandlerServiceFactory, ILogger<QueueJobCommandConsumer> logger, IWorkerClient workerClient)
        {
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            _jobHandlerServiceFactory = jobHandlerServiceFactory ??
                                        throw new ArgumentNullException(nameof(jobHandlerServiceFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _workerClient = workerClient ?? throw new ArgumentNullException(nameof(workerClient));
        }

        public async Task ConsumeAsync(QueueJobCommand queueJobCommand, CancellationToken cancellationToken = default)
        {
            var stepIndex = 0;
            var jobDetails = GetJobDetails(queueJobCommand);

            try
            {
                _logger.LogInformation($"Starting {queueJobCommand.JobConfiguration.Context.ToString()} job: {jobDetails.JobId} (Run: {jobDetails.RunId})...");
                await ProcessJobAsync(jobDetails, queueJobCommand.JobConfiguration, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Job was cancelled remotely.");
                await _workerClient.SendStepResult(new StepResultEvent
                {
                    State = States.Cancelled,
                    StepIndex = stepIndex,
                    Token = queueJobCommand.Token
                });
            }
            catch (StepFailedException exception)
            {
                _logger.LogError($"Step {exception.Name} failed. Exit code was {exception.ExitCode}.");
                await _workerClient.SendStepResult(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = stepIndex,
                    Token = queueJobCommand.Token
                });
            }
            catch (Exception exception)
            {
                _logger.LogError($"An unexpected error has occured. Error: {exception.Message}");
                await _workerClient.SendStepResult(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = stepIndex,
                    Token = queueJobCommand.Token
                });
                // rest will be marked as skipped
            }
            finally
            {
                _logger.LogInformation($"Finished job: {jobDetails.JobId} (Run: {jobDetails.RunId}).");
            }
        }

        private async Task ProcessJobAsync(JobDetails jobDetails, JobConfiguration jobConfiguration, CancellationToken cancellationToken = default)
        {
            // TODO: only set agent state server side?
            _agentService.AgentState = AgentState.Working;
            var beforeExecutionDateTime = DateTime.Now;
            
            await using var jobHandler = _jobHandlerServiceFactory.Create(jobDetails,
                jobConfiguration,
                cancellationToken);

            await jobHandler.PrepareEnvironmentAsync();

            for (var i = 0; i < jobConfiguration.Steps.Count; i++)
            {
                var channel = Channel.CreateUnbounded<string>();

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _ = _workerClient.StreamLogAsync(jobDetails.RunId, jobDetails.RunId, i,
                        channel.Reader); // do not await!

                    await ProcessStepAsync(channel, i, jobHandler, jobConfiguration.Steps[i], jobDetails);

                }
                finally  // try-finally to make sure the channel write gets completed
                {
                    channel.Writer.Complete();
                }
            }
            
            var afterExecutionDateTime = DateTime.Now;
            var totalTime = afterExecutionDateTime - beforeExecutionDateTime;

            _logger.LogInformation($"Job ran for {totalTime.TotalSeconds} second(s).");
            
        }
        
        private async Task ProcessStepAsync(ChannelWriter<string> channelWriter, int stepIndex, IJobHandlerService jobHandlerService,StepConfiguration step, JobDetails jobDetails)
        {
            _logger.LogInformation($"Executing step {step.Name}...");
            
            await _workerClient.SendStepResult(new StepResultEvent
            {
                State = States.Running,
                StepIndex = stepIndex,
                Token = jobDetails.Token
            });

            await jobHandlerService.ExecuteStepAsync(channelWriter, step, stepIndex);

            _logger.LogInformation($"Successfully executed step {step.Name}.");
            await _workerClient.SendStepResult(new StepResultEvent
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
                .FirstOrDefault(claim => claim.Type == CustomJwtRegisteredClaimNames.RunId)?.Value!);
            var jobId = long.Parse(decodedToken.Claims
                .FirstOrDefault(claim => claim.Type == CustomJwtRegisteredClaimNames.JobId)?.Value!);

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