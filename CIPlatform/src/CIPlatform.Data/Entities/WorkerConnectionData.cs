using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;

namespace CIPlatform.Data.Entities
{
    public class WorkerConnectionData
    {
        public string ConnectionId { get; set; }
        public JobContext JobContext { get; set; }
        public AgentState AgentState { get; set; }
    }
}