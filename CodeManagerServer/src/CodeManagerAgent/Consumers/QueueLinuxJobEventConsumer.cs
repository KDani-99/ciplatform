using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.Consumers
{
    public class QueueLinuxJobEventConsumer : QueueJobEventConsumer, IConsumer<QueueLinuxJobEvent>
    {
        public QueueLinuxJobEventConsumer(IAgentService agentService, IRequestClient<RequestJobCommand> requestClient,
            IJobHandlerServiceFactory jobHandlerServiceFactory, ILogger<QueueJobEventConsumer> logger)
            : base(agentService, requestClient, jobHandlerServiceFactory, logger)
        {
        }

        public Task Consume(ConsumeContext<QueueLinuxJobEvent> context)
        {
            return base.Consume(context.Message);
        }
    }
}