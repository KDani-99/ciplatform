
using CodeManager.Data.Configuration;

namespace CodeManagerAgent.Configuration
{
    public class AgentConfiguration
    {
        public JobContext Context { get; set; }
        public string LogDirectory { get; set; }
    }
}