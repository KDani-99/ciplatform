using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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

        public override async Task OnConnectedAsync()
        {
            var x = Context.Features;
        }

        [HubMethodName("SubscribeToStepResultChannel")]
        public Task SubscribeToStepResultChannelAsync(long runId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.AddToGroupAsync(Context.ConnectionId, GetJobGroupName(runId));
        }
        
        [HubMethodName("UnSubscribeFromStepResultChannel")]
        public Task UnSubscribeFromStepResultChannelAsync(long runId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetJobGroupName(runId));
        }
        
        [HubMethodName("SubscribeToChannel")]
        public Task SubscribeToChannelAsync(long runId, long jobId)
        {
            // TODO: verify whether the user is allowed to see the run details
            return Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(runId, jobId));
        }

        [HubMethodName("UnsubscribeFromChannel")]
        public Task UnsubscribeFromChannelAsync(long runId, long jobId)
        {
            Groups.RemoveFromGroupAsync(Context.ConnectionId,
                                        GetGroupName(
                                            runId,
                                            jobId)); // In general, you should not include await when calling the Groups.Remove method because the connection id that you are trying to remove might no longer be available. 
            return Task.CompletedTask;
        }

        [HubMethodName("StreamLogToChannel")]
        public async Task SendLogsToChannelAsync(IAsyncEnumerable<string> stream,
                                                 long runId,
                                                 long jobId,
                                                 int step,
                                                 [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in stream.WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Yield();
                // TEST:
                Console.WriteLine("ASD => " + item);
                await Clients.All.SendAsync("ReceiveLogs", item, cancellationToken);
                // await Clients.Group(GetGroupName(runId, jobId)).SendAsync("ReceiveLogs", item, cancellationToken: cancellationToken);
            }
        }

        public static string GetGroupName(long runId, long jobId)
        {
            return $"{runId.ToString()}/{jobId.ToString()}";
        }

        public static string GetJobGroupName(long jobId)
        {
            return $"Job:{jobId}";
        }
    }
}