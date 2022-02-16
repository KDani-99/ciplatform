using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.WebSocket.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CIPlatformWebApi.Consumers
{
    public class ProcessedRunResultEventConsumer : IConsumer<ProcessedRunResultEvent>
    {
        private readonly IHubContext<RunsHub> _runsHubContext;

        public ProcessedRunResultEventConsumer(IHubContext<RunsHub> runsHubContext)
        {
            _runsHubContext = runsHubContext ?? throw new ArgumentNullException(nameof(runsHubContext));
        }

        public Task Consume(ConsumeContext<ProcessedRunResultEvent> context)
        {
            return _runsHubContext.Clients.Group(RunsHub.AvailableResultsChannels["project"](context.Message.ProjectId))
                                  .SendAsync("ReceiveRunResultEvent", context.Message);
        }
    }
}