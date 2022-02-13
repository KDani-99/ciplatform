using System;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CodeManagerWebApi.Consumers
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
            return _runsHubContext.Clients.Group(RunsHub.GetJobGroupName(context.Message.JobId))
                                  .SendAsync("ReceiveStepResultEvent", context.Message);
        }
    }
}