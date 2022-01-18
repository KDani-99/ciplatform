using System.Threading.Tasks;
using CodeManager.Data.Messaging;
using CodeManagerAgentManager.Services;
using MassTransit;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CodeManagerAgentManager.Consumers
{
    public class StartRunRequestConsumer : IConsumer<StartRunRequest>
    {
        public async Task Consume(ConsumeContext<StartRunRequest> context)
        {
            // TODO: request agents
            

            await context.RespondAsync(); // TODO: respond with status => started/agent not available
        }
    }
}