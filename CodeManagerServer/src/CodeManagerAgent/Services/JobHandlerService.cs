using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Common;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.JsonWebTokens;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Exceptions;
using CodeManagerAgent.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Services
{
    public abstract class JobHandlerService : IJobHandlerService
    {
        private readonly AgentConfiguration _agentConfiguration;

        private readonly IWorkerClient _workerClient;
        private readonly string _repository;

        private readonly string _token;
        protected readonly IAgentService AgentService;
        protected readonly CancellationToken CancellationToken;

        protected readonly JobConfiguration JobConfiguration;
        protected readonly long JobId;

        protected readonly ILogger<JobHandlerService> Logger;

        // protected readonly ISendEndpoint SendEndpoint;
        protected readonly long RunId;

        protected JobHandlerService(
            string repository,
            string token,
            JobConfiguration jobConfiguration,
            IWorkerClient workerClient,
            IOptions<AgentConfiguration> agentConfiguration,
            IAgentService agentService,
            ILogger<JobHandlerService> logger,
            CancellationToken cancellationToken)
        {
            // TODO: read config file from mount
            // TODO: or start is remotely as a process and read logs? => could directly stream docker container logs
            _token = token ?? throw new ArgumentNullException(nameof(token));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _agentConfiguration =
                agentConfiguration.Value ?? throw new ArgumentNullException(nameof(agentConfiguration));
            _workerClient = workerClient ?? throw new ArgumentNullException(nameof(workerClient));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            AgentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            JobConfiguration = jobConfiguration ?? throw new ArgumentNullException(nameof(jobConfiguration));
            CancellationToken = cancellationToken;
            // SendEndpoint = busControl.GetSendEndpoint(responseAddress).Result;

            var decodedToken = _token.DecodeJwtToken();
            RunId = long.Parse(decodedToken.Claims
                .FirstOrDefault(claim => claim.Type == CustomJwtRegisteredClaimNames.RunId)?.Value!);
            JobId = long.Parse(decodedToken.Claims
                .FirstOrDefault(claim => claim.Type == CustomJwtRegisteredClaimNames.JobId)?.Value!);
        }

        public abstract ValueTask DisposeAsync();

        public abstract void Dispose();

        public virtual async Task StartAsync()
        {
            // Non zero exit code indicates failure @ the given job
            Logger.LogInformation($"Starting job: {JobId} (Run: {RunId})...");

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
            var step = -1; // indicated unknown

            try
            {
                // Add setup step
                JobConfiguration.Steps.Insert(0, new StepConfiguration
                {
                    Name = "Checkout repository (setup)",
                    Cmd = $"git clone {_repository} {_agentConfiguration.WorkingDirectory}"
                });

                for (step = 0; step < JobConfiguration.Steps.Count; step++)
                {
                    CancellationToken.ThrowIfCancellationRequested();

                    await SendEventAsync(new StepResultEvent
                    {
                        State = States.Running,
                        StepIndex = step
                    }, CommonAgentManagerHubMethods.StepResultEvent);

                    await ExecuteStepAsync(JobConfiguration.Steps[step], step);

                    await SendEventAsync(new StepResultEvent
                    {
                        State = States.Successful,
                        StepIndex = step
                    }, CommonAgentManagerHubMethods.StepResultEvent);
                }
            }
            catch (OperationCanceledException exception)
            {
                Logger.LogError("Job was cancelled remotely.");
                await SendEventAsync(new StepResultEvent
                {
                    State = States.Cancelled,
                    StepIndex = step
                }, CommonAgentManagerHubMethods.StepResultEvent);
            }
            catch (StepFailedException exception)
            {
                Logger.LogError($"Step {exception.Name} failed. Exit code was {exception.ExitCode}.");
                await SendEventAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = step
                }, CommonAgentManagerHubMethods.StepResultEvent);
            }
            catch (Exception exception)
            {
                Logger.LogError($"An unexpected error has occured. Error: {exception.Message}");
                await SendEventAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = step
                }, CommonAgentManagerHubMethods.StepResultEvent);
                // rest will be marked as skipped
            }
            finally
            {
                Logger.LogInformation($"Finished job: {JobId} (Run: {RunId}).");
            }
        }

        protected abstract Task ExecuteStepAsync(StepConfiguration step, int stepIndex);

        protected async Task StreamLogAsync(int stepIndex, Func<IAsyncEnumerable<string>> stream)
        {
            await _workerClient.HubConnection.SendAsync("UploadLogStream", stream(), RunId, JobId, stepIndex);
        }

        protected Task SendEventAsync<TEvent>(TEvent @event, string hubMethod)
        {
            if (@event is ISecureMessage secureMessage) secureMessage.Token = _token; // TODO: token not required since signalr
            return _workerClient.HubConnection.SendAsync(hubMethod);
        }

        private string GetLogFilePath(int stepIndex)
        {
            return Path.Join(_agentConfiguration.LogDirectory, JobId.ToString(), $"step-{stepIndex}.log");
        }
    }
}