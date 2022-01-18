using System.Threading.Tasks;
using CodeManager.Data.Events;
using MassTransit;

namespace CodeManagerAgent.Consumers
{
    public class QueueJobEventConsumer : IConsumer<QueueJobEvent>
    {
        public Task Consume(ConsumeContext<QueueJobEvent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}