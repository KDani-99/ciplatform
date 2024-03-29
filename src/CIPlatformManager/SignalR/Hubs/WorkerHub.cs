﻿using System;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Events;
using CIPlatformManager.Services;
using CIPlatformManager.Services.Logs;
using CIPlatformManager.Services.Steps;
using CIPlatformManager.Services.Workers;
using CIPlatformManager.WebSocket;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CIPlatformManager.SignalR.Hubs
{
    public class WorkerHub : Hub<IWorkerClient>
    {
        private const string HeaderKey = "W-JobContext"; // W for Worker
        private readonly ILogger<WorkerHub> _logger;
        private readonly ILogStreamService _logStreamService;
        private readonly IStepService<StepResultEvent> _stepService;
        private readonly IWorkerConnectionService _workerConnectionService;

        public WorkerHub(IWorkerConnectionService workerConnectionService,
                        ILogStreamService logStreamService,
                        ILogger<WorkerHub> logger,
                        IStepService<StepResultEvent> stepService)
        {
            _workerConnectionService = workerConnectionService ??
                throw new ArgumentNullException(nameof(workerConnectionService));
            _logStreamService = logStreamService ?? throw new ArgumentNullException(nameof(logStreamService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stepService = stepService ?? throw new ArgumentNullException(nameof(stepService));
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var jobContextString = Context.GetHttpContext().Request.Headers
                                              .First(header => header.Key == HeaderKey).Value;

                if (!Enum.TryParse(jobContextString, out JobContext jobContext))
                    throw new ArgumentException($"Invalid value provided for '{HeaderKey}' header.");

                await _workerConnectionService.AddWorkerConnectionOfTypeAsync(new WorkerConnectionDataEntity
                {
                    WorkerState = WorkerState.Unavailable, // must be configured first
                    ConnectionId = Context.ConnectionId,
                    JobContext = jobContext,
                    LastPing = DateTime.Now
                });
                _logger.LogInformation($"Client '{Context.ConnectionId}' has connected.");
            }
            catch (Exception exception)
            {
                Context.Abort();
                _logger.LogError($"A client has tried to connect without a valid '{HeaderKey}' header. Message: " +
                                 exception.Message);
                return;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _workerConnectionService.RemoveWorkerConnectionAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("Ping")]
        public async Task PingAsync()
        {
            try
            {
                await _workerConnectionService.KeepWorkerConnectionAsync(Context.ConnectionId);
            }
            catch (Exception exception)
            {
                _logger.LogError($"An unexpected error has occured. Message: {exception.Message}.");
            }
        }

        [HubMethodName("FinishJob")]
        public async Task FinishJobAsync()
        {
            try
            {
                await _workerConnectionService.MarkWorkerConnectionAsAvailableAsync(Context.ConnectionId);
            }
            catch (Exception exception)
            {
                _logger.LogError($"An unexpected error has occured. Message: {exception.Message}.");
            }
        }

        [HubMethodName("Configure")]
        public async Task ConfigureWorkerAsync(WorkerState workerState)
        {
            try
            {
                await _workerConnectionService.UpdateWorkerConnectionAsync(new WorkerConnectionDataEntity
                {
                    WorkerState = workerState,
                    LastPing = DateTime.Now,
                    ConnectionId = Context.ConnectionId // jobcontext wont be updated
                });
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to configure worker. Error: {exception.Message}");
                Context.Abort();
            }
        }

        [HubMethodName("UpdateAgentState")]
        public Task UpdateAgentStateAsync(WorkerState workerState)
        {
            return workerState switch
            {
                WorkerState.Available => Groups.AddToGroupAsync(Context.ConnectionId, nameof(WorkerState.Available)),
                WorkerState.Working => Groups.RemoveFromGroupAsync(Context.ConnectionId, nameof(WorkerState.Working)),
                _ => throw new ArgumentOutOfRangeException(nameof(workerState))
            };
        }

        [HubMethodName("StepResultEvent")]
        public async Task ReceiveStepResultEventAsync(StepResultEvent stepResultEvent)
        {
            try
            {
                await _stepService.ProcessStepResultAsync(stepResultEvent, Context.ConnectionId);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to consume `{nameof(StepResultEvent)}`. Error: {exception.Message}");
            }
        }

        [HubMethodName("UploadLogStream")]
        public Task UploadLogStreamAsync(ChannelReader<string> stream, long runId, long jobId, int stepIndex)
        {
            try
            {
                return _logStreamService.ProcessStreamAsync(stream, runId, jobId, stepIndex);
            }
            catch (Exception exception)
            {
                _logger.LogError($"An unexpected error has occurred. Message: {exception.Message}.");
            }

            return Task.CompletedTask;
        }
    }
}