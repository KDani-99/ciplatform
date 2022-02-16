using CIPlatform.Data.Configuration;

namespace CIPlatformWorker.Configuration
{
    public class AgentConfiguration
    {
        public JobContext Context { get; set; }
        public string WorkingDirectory { get; set; }
    }
}