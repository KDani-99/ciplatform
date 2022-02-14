using System;
using System.Threading.Tasks;
using CodeManager.Data.Events;
using CodeManagerWebApi.Services;
using CodeManagerWebApi.WebSocket.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CodeManagerWebApi.Consumers
{
    public class RunQueuedEventConsumer : IConsumer<RunQueuedEvent>
    {
        private readonly IRunService _runService;
        private readonly IHubContext<RunsHub> _runsHubContext;

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