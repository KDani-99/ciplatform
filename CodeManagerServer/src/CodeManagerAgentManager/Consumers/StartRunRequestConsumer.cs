using System.Threading.Tasks;
using CodeManager.Data.Messaging;
using MassTransit;

namespace CodeManagerAgentManager.Consumers
{
    public class StartRunRequestConsumer : IConsumer<StartRunRequest>
    {
        
        
        public async Task Consume(ConsumeContext<StartRunRequest> context)
        {
            // TODO: request agents
            

            await context.RespondAsync(); // TODO: responde with status => started/agent not available
        }
    }
}