using System;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.WebSocket.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CodeManagerWebApi.Consumers
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
            return _runsHubContext.Clients.Group(RunsHub.GetProjectGroupName(context.Message.ProjectId))
                                  .SendAsync("ReceiveRunResultEvent", context.Message);
        }
    }
}