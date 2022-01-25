using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.Consumers
{
    public class QueueDockerJobEventConsumer : QueueJobEventConsumer, IConsumer<QueueDockerJobEvent>
    {
        public QueueDockerJobEventConsumer(IAgentService agentService, IRequestClient<RequestJobCommand> requestClient,
            IJobHandlerServiceFactory jobHandlerServiceFactory, ILogger<QueueJobEventConsumer> logger)
            : base(agentService, requestClient, jobHandlerServiceFactory, logger)
        {
        }

        public Task Consume(ConsumeContext<QueueDockerJobEvent> context)
        {
            return base.Consume(context.Message);
        }
    }
}