using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CIPlatformWebApi.WebSocket.Hubs
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
        public async Task StreamLogsToChannelAsync(ChannelReader<string> stream, long stepId)
        {
            try
            {
                while (await stream.WaitToReadAsync())
                {
                    while (stream.TryRead(out var item))
                    {
                        await _runsHubContext.Clients.Group(RunsHub.AvailableResultsChannels["step"](stepId))
                            .SendAsync("ReceiveLogs", item);
                        //await _runsHubContext.Clients.Group(GetGroupName(runId, jobId)).SendAsync("ReceiveLogs", item); // why send? because thats the only way to ""stream"" it to multiple clients
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"An unexpected error has occured. Message : {exception.Message}.");
            }
        }
    }
}