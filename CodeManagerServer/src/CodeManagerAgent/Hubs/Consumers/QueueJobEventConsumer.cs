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

namespace CodeManagerAgent.Hubs.Consumers
{
    public class QueueJobEventConsumer : IConsumer<QueueJobEvent>
    {
        private readonly IAgentService _agentService;
        private readonly IJobHandlerServiceFactory _jobHandlerServiceFactory;

        public QueueJobEventConsumer(IAgentService agentService,
            IJobHandlerServiceFactory jobHandlerServiceFactory)
        {
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            _jobHandlerServiceFactory = jobHandlerServiceFactory ??
                                        throw new ArgumentNullException(nameof(jobHandlerServiceFactory));
        }

        public async Task Consume(QueueJobEvent queueJobEvent)
        {
            try
            {
                // TODO: only set agent state server side?
                _agentService.AgentState = AgentState.Working;
            
                _agentService.CancellationTokenSource = new CancellationTokenSource();

                await _jobHandlerServiceFactory.Create(queueJobEvent.Repository,
                        queueJobEvent.Token,
                        queueJobEvent.JobConfiguration,
                        _agentService.CancellationTokenSource.Token)
                    .StartAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}