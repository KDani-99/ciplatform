using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Data.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CodeManagerWebApi.Hubs
{
    public sealed class RunsHub : Hub
    {
        private readonly ILogger<RunsHub> _logger;
        
        public RunsHub(ILogger<RunsHub> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HubMethodName("SubscribeToChannel")]
        public Task SubscribeToChannelAsync(long runId, long jobId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.AddToGroupAsync(Context.ConnectionId, $"{runId}/{jobId}");
        }

        [HubMethodName("UnsubscribeFromChannel")]
        public Task UnsubscribeFromChannelAsync(long runId, long jobId)
        {
            Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{runId}/{jobId}"); // In general, you should not include await when calling the Groups.Remove method because the connection id that you are trying to remove might no longer be available. 
            return Task.CompletedTask;
        }

        /*[HubMethodName("StreamLogs")]
        public async IAsyncEnumerable<string> StreamLogsAsync(IAsyncEnumerable<string> stream, long runId, long jobId, int step, [EnumeratorCancellation]
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Clients.All.SendCoreAsync("", stream, runId);
            // stream it back to the client

        }*/
    }
}