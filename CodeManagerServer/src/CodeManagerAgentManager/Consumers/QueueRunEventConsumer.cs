using System.Threading.Tasks;
using MassTransit;

namespace CodeManagerAgentManager.Consumers
{
    public class QueueRunEventConsumer : IConsumer<QueueRunEventConsumer>
    {
        public Task Consume(ConsumeContext<QueueRunEventConsumer> context)
        {
            // Received from the WebAPI
            throw new System.NotImplementedException();
        }
    }
}