using System.Threading;
using CodeManager.Data.Agent;

namespace CodeManagerAgent.Services
{
    public interface IAgentService
    {
        public AgentState AgentState { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}