using System.Threading.Tasks;
using CodeManagerAgentManager.Commands;
using MassTransit;

namespace CodeManagerAgentManager.Consumers
{
    public class StartJobCommandConsumer : IConsumer<StartJobCommand>
    {
        public Task Consume(ConsumeContext<StartJobCommand> context)
        {
            throw new System.NotImplementedException();
        }
    }
}