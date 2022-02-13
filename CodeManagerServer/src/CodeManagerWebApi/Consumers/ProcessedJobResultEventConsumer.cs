using System;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CodeManagerWebApi.Consumers
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
            return _runsHubContext.Clients.Group(RunsHub.GetRunGroupName(context.Message.RunId))
                                  .SendAsync("ReceiveJobResultEvent", context.Message);
        }
    }
}