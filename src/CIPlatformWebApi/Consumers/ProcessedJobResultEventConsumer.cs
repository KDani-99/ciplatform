using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.WebSocket.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CIPlatformWebApi.Consumers
{
    public class ProcessedJobResultEventConsumer : IConsumer<ProcessedJobResultEvent>
    {
        private readonly IHubContext<RunsHub> _runsHubContext;

        public ProcessedJobResultEventConsumer(IHubContext<RunsHub> runsHubContext)
        {
            _runsHubContext = runsHubContext ?? throw new ArgumentNullException(nameof(runsHubContext));
        }

        public Task Consume(ConsumeContext<ProcessedJobResultEvent> context)
        {
            return _runsHubContext.Clients.Group(RunsHub.AvailableResultsChannels["run"](context.Message.RunId))
                                  .SendAsync("ReceiveJobResultEvent", context.Message);
        }
    }
}