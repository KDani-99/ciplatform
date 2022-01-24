using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using MassTransit;

namespace CodeManagerAgent.Hubs.Consumers
{
    public class QueueWindowsJobEventConsumer : QueueJobEventConsumer, IConsumer<QueueWindowsJobEvent>
    {
        public QueueWindowsJobEventConsumer(IAgentService agentService, IRequestClient<RequestJobCommand> requestClient, IJobHandlerServiceFactory jobHandlerServiceFactory)
            : base(agentService, requestClient, jobHandlerServiceFactory)
        {
        }

        public Task Consume(QueueWindowsJobEvent context)
        {
            return base.Consume(context);
        }
    }
}