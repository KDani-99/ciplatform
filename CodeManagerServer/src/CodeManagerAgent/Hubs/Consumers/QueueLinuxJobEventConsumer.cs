using System.Threading.Tasks;
using CodeManager.Core.Hubs.Consumers;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerAgent.Hubs.Consumers
{
    public class QueueLinuxJobEventConsumer : QueueJobEventConsumer, IConsumer<QueueLinuxJobEvent>
    {
        public QueueLinuxJobEventConsumer(HubConnection hubConnection, IAgentService agentService,
            IJobHandlerServiceFactory jobHandlerServiceFactory)
            : base(hubConnection, agentService, jobHandlerServiceFactory)
        {
        }

        public Task Consume(QueueLinuxJobEvent context)
        {
            return base.Consume(context);
        }
    }
}