using CIPlatform.Data.Configuration;

namespace CIPlatformWorker.Configuration
{
    public class WorkerConfiguration
    {
        public JobContext Context { get; set; }
        public string WorkingDirectory { get; set; }
    }
}