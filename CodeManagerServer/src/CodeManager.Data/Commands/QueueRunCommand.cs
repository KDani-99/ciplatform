using CodeManager.Data.Configuration;

namespace CodeManager.Data.Commands
{
    public class QueueRunCommand
    {
        public string Repository { get; init; }
        public string RunConfigurationString { get; init; }
        public long ProjectId { get; init; }
    }
}