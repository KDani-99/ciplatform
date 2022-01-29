using System.Threading.Tasks;
using CodeManager.Core.Hubs.Consumers;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerAgent.Hubs.Consumers
{
    public class QueueWindowsJobEventConsumer : QueueJobEventConsumer, IConsumer<QueueWindowsJobEvent>
    {
        public QueueWindowsJobEventConsumer(HubConnection hubConnection, IAgentService agentService,
            IJobHandlerServiceFactory jobHandlerServiceFactory)
            : base(hubConnection, agentService, jobHandlerServiceFactory)
        {
        }

        public Task Consume(QueueWindowsJobEvent context)
        {
            return base.Consume(context);
        }
    }
}