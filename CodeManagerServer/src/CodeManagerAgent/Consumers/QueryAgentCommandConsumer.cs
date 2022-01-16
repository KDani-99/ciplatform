using System.Threading.Tasks;
using CodeManager.Data.Messaging;
using MassTransit;

namespace CodeManagerAgent.Consumers
{
    public class QueryAgentCommandConsumer : IConsumer<QueryAgentCommand>
    {
        public Task Consume(ConsumeContext<QueryAgentCommand> context)
        {
            throw new System.NotImplementedException();
        }
    }
}