using System.Threading.Tasks;
using CodeManager.Data.Agent;
using MassTransit;

namespace CodeManagerAgent.Services
{
    public interface IAgentService
    {
        public AgentState AgentState { get; set; }
    }
}