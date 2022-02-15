using CodeManager.Data.Agent;
using CodeManager.Data.Configuration;

namespace CodeManager.Data.Entities
{
    public class WorkerConnectionData
    {
        public string ConnectionId { get; set; }
        public JobContext JobContext { get; set; }
        public AgentState AgentState { get; set; }
    }
}