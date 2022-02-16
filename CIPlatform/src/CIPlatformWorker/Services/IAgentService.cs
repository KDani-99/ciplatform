using System.Threading;
using CIPlatform.Data.Agent;

namespace CIPlatformWorker.Services
{
    public interface IAgentService
    {
        public AgentState AgentState { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}