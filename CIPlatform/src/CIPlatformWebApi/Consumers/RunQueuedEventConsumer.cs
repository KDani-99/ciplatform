using System;
using System.Threading.Tasks;
using CIPlatform.Data.Events;
using CIPlatformWebApi.Services;
using CIPlatformWebApi.WebSocket.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CIPlatformWebApi.Consumers
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
            await _runsHubContext.Clients.Group(RunsHub.AvailableResultsChannels["project"](context.Message.ProjectId))
                                 .SendAsync("ReceiveRunQueuedEvent", runDto);
        }
    }
}