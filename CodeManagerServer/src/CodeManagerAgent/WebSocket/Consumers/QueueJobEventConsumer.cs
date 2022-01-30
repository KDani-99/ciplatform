using System;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Common;
using CodeManager.Core.Hubs.Consumers;
using CodeManager.Data.Agent;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.WebSocket.Consumers
{
    public class QueueJobEventConsumer : IConsumer<QueueJobEvent>
    {
        private readonly IAgentService _agentService;
        private readonly IJobHandlerServiceFactory _jobHandlerServiceFactory;
        private readonly ILogger<QueueJobEventConsumer> _logger;

        public QueueJobEventConsumer(IAgentService agentService,
            IJobHandlerServiceFactory jobHandlerServiceFactory, ILogger<QueueJobEventConsumer> logger)
        {
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            _jobHandlerServiceFactory = jobHandlerServiceFactory ??
                                        throw new ArgumentNullException(nameof(jobHandlerServiceFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(QueueJobEvent queueJobEvent)
        {
            try
            {
                // TODO: only set agent state server side?
                _agentService.AgentState = AgentState.Working;
            
                _agentService.CancellationTokenSource = new CancellationTokenSource();

                using var jobHandle =  _jobHandlerServiceFactory.Create(queueJobEvent.Repository,
                    queueJobEvent.Token,
                    queueJobEvent.JobConfiguration,
                    _agentService.CancellationTokenSource.Token).StartAsync(); // to make sure to dispose it!!
            }
            catch (Exception exception)
            {
                _logger.LogError("An unexpected error has occurred. Error: " + exception.Message);
            }
        }
    }
}