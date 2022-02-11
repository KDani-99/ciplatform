using System;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CodeManagerWebApi.Consumers
{
    public class ProcessedStepResultEventConsumer : IConsumer<ProcessedStepResult>
    {
        private readonly IHubContext<RunsHub> _runsHubContext;

        public ProcessedStepResultEventConsumer(IHubContext<RunsHub> runsHubContext)
        {
            _runsHubContext = runsHubContext ?? throw new ArgumentNullException(nameof(runsHubContext));
        }
        public async Task Consume(ConsumeContext<ProcessedStepResult> context)
        {
            _runsHubContext.Clients.Group(context.Message.ProjectId.ToString(), )
        }
    }
}