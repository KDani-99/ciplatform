using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.WebSocket.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CIPlatformWebApi.Consumers
{
    public class ProcessedStepResultEventConsumer : IConsumer<ProcessedStepResultEvent>
    {
        private readonly IHubContext<RunsHub> _runsHubContext;

        public ProcessedStepResultEventConsumer(IHubContext<RunsHub> runsHubContext)
        {
            _runsHubContext = runsHubContext ?? throw new ArgumentNullException(nameof(runsHubContext));
        }

        public Task Consume(ConsumeContext<ProcessedStepResultEvent> context)
        {
            return _runsHubContext.Clients.Group(RunsHub.AvailableResultsChannels["job"](context.Message.JobId))
                                  .SendAsync("ReceiveStepResultEvent", context.Message);
        }
    }
}