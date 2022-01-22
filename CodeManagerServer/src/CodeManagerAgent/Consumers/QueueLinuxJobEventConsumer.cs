using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using MassTransit;

namespace CodeManagerAgent.Consumers
{
    public class QueueLinuxJobEventConsumer : QueueJobEventConsumer, IConsumer<QueueLinuxJobEvent>
    {
        public QueueLinuxJobEventConsumer(IAgentService agentService, IRequestClient<RequestJobCommand> requestClient, IJobHandlerServiceFactory jobHandlerServiceFactory)
            : base(agentService, requestClient, jobHandlerServiceFactory)
        {
        }
    }
}