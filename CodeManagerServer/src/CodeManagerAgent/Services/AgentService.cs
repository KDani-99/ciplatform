using System.Threading.Tasks;
using CodeManager.Data.Agent;
using CodeManager.Data.Events;
using MassTransit;

namespace CodeManagerAgent.Services
{
    public class AgentService : IAgentService
    {
        //Singleton
        public AgentState AgentState { get; set; }
        public string Token { get; set; }
        
        public Task SendAsync<T>(T message, ISendEndpoint sendEndpoint)
            where T : new()
        {
            if (message is ISecureMessage secureMessage)
            {
                secureMessage.Token = Token;
            }

            return sendEndpoint.Send(message);
        }
    }
}