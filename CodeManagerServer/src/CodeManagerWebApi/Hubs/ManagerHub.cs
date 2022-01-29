using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CodeManagerWebApi.Hubs
{
    public class ManagerHub : Hub
    {
        private readonly ILogger<ManagerHub> _logger;
        private readonly IHubContext<RunsHub> _runsHubContext;
        
        public ManagerHub(ILogger<ManagerHub> logger, IHubContext<RunsHub> runsHubContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _runsHubContext = runsHubContext ?? throw new ArgumentNullException(nameof(runsHubContext));
        }
        
        [HubMethodName("StreamLogToChannel")]
        public async Task SendLogsToChannelAsync(IAsyncEnumerable<string> stream, long runId, long jobId, int step, [EnumeratorCancellation]
            CancellationToken cancellationToken)
        {
            await foreach (var item in stream.WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await Task.Yield();
                await _runsHubContext.Clients.Group(GetGroupName(runId, jobId)).SendAsync("ReceiveLogs", item, cancellationToken: cancellationToken);
            }

        }
        
        private static string GetGroupName(long runId, long jobId)
        {
            return $"{runId.ToString()}/{jobId.ToString()}";
        }
    }
}