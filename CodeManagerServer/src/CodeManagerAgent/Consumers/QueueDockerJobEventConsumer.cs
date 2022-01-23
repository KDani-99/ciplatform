using System;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using MassTransit;

namespace CodeManagerAgent.Consumers
{
    public class QueueDockerJobEventConsumer : QueueJobEventConsumer, IConsumer<QueueDockerJobEvent>
    {
        public QueueDockerJobEventConsumer(IAgentService agentService, IRequestClient<RequestJobCommand> requestClient, IJobHandlerServiceFactory jobHandlerServiceFactory)
            : base(agentService, requestClient, jobHandlerServiceFactory)
        {
        }

        public Task Consume(ConsumeContext<QueueDockerJobEvent> context)
        {
            return base.Consume(context.Message);
        }
    }
}