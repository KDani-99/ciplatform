using System;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManager.Data.Events;
using CodeManagerWebApi.Hubs;
using CodeManagerWebApi.Services;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CodeManagerWebApi.Consumers
{
    public class RunQueuedEventConsumer : IConsumer<RunQueuedEvent>
    {
        private readonly IHubContext<RunsHub> _runsHubContext;
        private readonly IRunService _runService;

        public RunQueuedEventConsumer(IHubContext<RunsHub> runsHubContext, IRunService runService)
        {
            _runsHubContext = runsHubContext ?? throw new ArgumentNullException(nameof(runsHubContext));
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
        }
        public async Task Consume(ConsumeContext<RunQueuedEvent> context)
        {
            var runDto = await _runService.GetRunAsync(context.Message.RunId);
            await _runsHubContext.Clients.Group(RunsHub.GetProjectGroupName(context.Message.ProjectId))
                                  .SendAsync("ReceiveRunQueuedEvent", runDto);
        }
    }
}