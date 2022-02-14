using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CodeManagerWebApi.Exceptions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CodeManagerWebApi.Hubs
{
    public sealed class RunsHub : Hub
    {
        private readonly ILogger<RunsHub> _logger;
        private readonly IReadOnlyDictionary<string, Func<long, string>> _availableResultChannels;

        public RunsHub(ILogger<RunsHub> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _availableResultChannels = new Dictionary<string, Func<long, string>>
            {
                {"project", (entityId) => $"Project:{entityId}"},
                {"run", (entityId) => $"Run:{entityId}"},
                {"job", (entityId) => $"Job:{entityId}"},
                {"step", (entityId) => $"Step:{entityId}"},
            };
        }

        public override async Task OnConnectedAsync()
        {
            var x = Context.Features;
        }

        [HubMethodName("SubscribeToResultsChannel")]
        public Task SubscribeToResultsChannelAsync(long entityId, string resultsChannel)
        {
            // TODO: verify whether the user is allowed to see the run details
            // TODO: design pattern to verify? startegy?

            if (!_availableResultChannels.ContainsKey(resultsChannel))
            {
                throw new ChannelDoesNotExistException(resultsChannel);
            }

            return Groups.AddToGroupAsync(Context.ConnectionId, _availableResultChannels[resultsChannel](entityId));
        }

        [HubMethodName("SubscribeToStepResultChannel")]
        public Task SubscribeToStepResultChannelAsync(long jobId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.AddToGroupAsync(Context.ConnectionId, GetJobGroupName(jobId));
        }
        
        [HubMethodName("UnSubscribeFromStepResultChannel")]
        public Task UnSubscribeFromStepResultChannelAsync(long jobId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetJobGroupName(jobId));
        }
        
        [HubMethodName("SubscribeToJobResultChannel")]
        public Task SubscribeToJobResultChannelAsync(long runId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.AddToGroupAsync(Context.ConnectionId, GetRunGroupName(runId));
        }
        
        [HubMethodName("UnSubscribeFromJobResultChannel")]
        public Task UnSubscribeFromJobResultChannelAsync(long runId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetRunGroupName(runId));
        }
        
        [HubMethodName("SubscribeToRunResultChannel")]
        public Task SubscribeToRunResultChannelAsync(long projectId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.AddToGroupAsync(Context.ConnectionId, GetProjectGroupName(projectId));
        }
        
        [HubMethodName("UnSubscribeFromRunResultChannel")]
        public Task UnSubscribeFromRunResultChannelAsync(long projectId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetProjectGroupName(projectId));
        }
        
        [HubMethodName("SubscribeToLogsChannel")]
        public Task SubscribeToLogsChannelAsync(long stepId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.AddToGroupAsync(Context.ConnectionId, GetStepGroupName(stepId));
        }
        
        [HubMethodName("UnSubscribeFromLogsChannel")]
        public Task UnSubscribeFromLogsChannelAsync(long stepId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetStepGroupName(stepId));
        }

        public static string GetStepGroupName(long stepId)
        {
            return $"Step:{stepId}";
        }

        public static string GetJobGroupName(long jobId)
        {
            return $"Job:{jobId}";
        }

        public static string GetRunGroupName(long runId)
        {
            return $"Run:{runId}";
        }
        
        public static string GetProjectGroupName(long runId)
        {
            return $"Project:{runId}";
        }
    }
}