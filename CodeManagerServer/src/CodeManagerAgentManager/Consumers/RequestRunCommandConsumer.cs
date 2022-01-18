using System.Threading.Tasks;
using CodeManager.Data.Commands;
using MassTransit;

namespace CodeManagerAgentManager.Consumers
{
    public class RequestRunCommandConsumer : IConsumer<RequestRunCommand>
    {
        public Task Consume(ConsumeContext<RequestRunCommand> context)
        {
            // Received from a connected worker (agent)
            // RPC
            
            throw new System.NotImplementedException();
        }
    }
}