using System.Threading.Tasks;
using CodeManager.Core.Hubs.Consumers;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerAgent.Hubs.Consumers
{
    public class QueueDockerJobEventConsumer : QueueJobEventConsumer, IConsumer<QueueJobEvent>
    {
        public QueueDockerJobEventConsumer(IAgentService agentService,
            IJobHandlerServiceFactory jobHandlerServiceFactory)
            : base(agentService, jobHandlerServiceFactory)
        {
        }

        public Task Consume(QueueJobEvent context)
        {
            return base.Consume(context);
        }
    }
}