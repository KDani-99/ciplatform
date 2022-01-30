using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
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
        
       /* [HubMethodName("StreamLogToChannel")]
        public async Task SendLogsToChannelAsync(IAsyncEnumerable<string> stream, long runId, long jobId, int step)
        {
            await foreach (var item in stream)
            {
                Console.Write(item);
                await Task.Yield();
                await _runsHubContext.Clients.Group(GetGroupName(runId, jobId)).SendAsync("ReceiveLogs", item);
            }

        }*/

       [HubMethodName("StreamLogToChannel")]
       public async Task StreamLogsToChannelAsync(ChannelReader<string> stream, long runId, long jobId, int step)
       {
           while (await stream.WaitToReadAsync())
           {
               while (stream.TryRead(out var item))
               {
                   Console.Write(item);
                   await _runsHubContext.Clients.Group(GetGroupName(runId, jobId)).SendAsync("ReceiveLogs", item); // why send? because thats the only way to ""stream"" it to multiple clients
                   // as regular streaming involves only a single client
               }
           }
       }

       [HubMethodName("NotifyStepResult")]
       public async Task NotifyStepResultAsync(ProcessedStepResult processedStepResult)
       {
           // TODO:
       }

       [HubMethodName("NotifyJobQueueResponse")]
       public async Task NotifyJobQueueResponseAsync(ProcessedJobRequest processedJobRequest)
       {
           // TODO:
       }
        
        private static string GetGroupName(long runId, long jobId)
        {
            return $"{runId.ToString()}/{jobId.ToString()}";
        }
    }
}