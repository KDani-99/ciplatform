using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Common;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities.CI;
using CodeManager.Data.Events;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Exceptions;
using CodeManagerAgent.WebSocket;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Services
{
    public abstract class JobHandlerService : IJobHandlerService
    {
        private readonly AgentConfiguration _agentConfiguration;
        private readonly IWorkerClient _workerClient;
        protected readonly CancellationToken CancellationToken;
        protected readonly JobConfiguration JobConfiguration;

        protected readonly JobDetails JobDetails;
        protected readonly ILogger<JobHandlerService> Logger;

        protected JobHandlerService(
            JobDetails jobDetails,
            JobConfiguration jobConfiguration,
            IWorkerClient workerClient,
            IOptions<AgentConfiguration> agentConfiguration,
            ILogger<JobHandlerService> logger,
            CancellationToken cancellationToken)
        {
            JobDetails = jobDetails ?? throw new ArgumentNullException(nameof(jobDetails));
            _agentConfiguration =
                agentConfiguration.Value ?? throw new ArgumentNullException(nameof(agentConfiguration));
            _workerClient = workerClient ?? throw new ArgumentNullException(nameof(workerClient));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            JobConfiguration = jobConfiguration ?? throw new ArgumentNullException(nameof(jobConfiguration));
            CancellationToken = cancellationToken;
        }

        public abstract ValueTask DisposeAsync();

        public abstract void Dispose();

        public virtual async Task StartAsync()
        {
            // Non zero exit code indicates failure @ the given job
            Logger.LogInformation($"Starting job: {JobDetails.JobId} (Run: {JobDetails.RunId})...");

            var beforeExecutionDateTime = DateTime.Now;
            await ExecuteJobAsync();
            var afterExecutionDateTime = DateTime.Now;

            var seconds = afterExecutionDateTime - beforeExecutionDateTime;
            // TODO: send metrics
            Logger.LogInformation($"Job ran for {seconds.TotalSeconds} second(s).");
        }

        protected async Task ExecuteJobAsync()
        {
            // TODO: signal job start
            var stepIndex = -1; // indicated unknown

            try
            {
                // Add setup step
                JobConfiguration.Steps.Insert(0, new StepConfiguration
                {
                    Name = "Checkout repository (setup)",
                    Cmd = $"git clone {JobDetails.Repository} {_agentConfiguration.WorkingDirectory}"
                });

                for (stepIndex = 0; stepIndex < JobConfiguration.Steps.Count; stepIndex++)
                {
                    CancellationToken.ThrowIfCancellationRequested();
                    await ProcessStepAsync(stepIndex, JobConfiguration.Steps[stepIndex]);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.LogError("Job was cancelled remotely.");
                await SendEventAsync(new StepResultEvent
                {
                    State = States.Cancelled,
                    StepIndex = stepIndex
                }, CommonAgentManagerHubMethods.StepResultEvent);
            }
            catch (StepFailedException exception)
            {
                Logger.LogError($"Step {exception.Name} failed. Exit code was {exception.ExitCode}.");
                await SendEventAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = stepIndex
                }, CommonAgentManagerHubMethods.StepResultEvent);
            }
            catch (Exception exception)
            {
                Logger.LogError($"An unexpected error has occured. Error: {exception.Message}");
                await SendEventAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = stepIndex
                }, CommonAgentManagerHubMethods.StepResultEvent);
                // rest will be marked as skipped
            }
            finally
            {
                Logger.LogInformation($"Finished job: {JobDetails.JobId} (Run: {JobDetails.RunId}).");
            }
        }

        protected abstract Task ExecuteStepAsync(StepConfiguration step, int stepIndex);

        protected async Task StreamLogAsync(int stepIndex, Func<IAsyncEnumerable<string>> stream)
        {
            await _workerClient.HubConnection.SendAsync("UploadLogStream", stream(), JobDetails.RunId, JobDetails.JobId,
                stepIndex);
        }

        protected Task SendEventAsync<TEvent>(TEvent @event, string hubMethod)
        {
            if (@event is ISecureMessage secureMessage)
                secureMessage.Token = JobDetails.Token; // TODO: token not required since signalr
            return _workerClient.HubConnection.SendAsync(hubMethod, @event);
        }

        private async Task ProcessStepAsync(int stepIndex, StepConfiguration step)
        {
            Logger.LogInformation($"Executing step {step.Name}...");
            await SendEventAsync(new StepResultEvent
            {
                State = States.Running,
                StepIndex = stepIndex
            }, CommonAgentManagerHubMethods.StepResultEvent);

            await ExecuteStepAsync(step, stepIndex);

            Logger.LogInformation($"Successfully executed step {step.Name}.");
            await SendEventAsync(new StepResultEvent
            {
                State = States.Successful,
                StepIndex = stepIndex
            }, CommonAgentManagerHubMethods.StepResultEvent);
        }
    }
}