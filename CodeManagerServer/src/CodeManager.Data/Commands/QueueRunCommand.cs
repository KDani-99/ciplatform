using CodeManager.Data.Configuration;

namespace CodeManager.Data.Commands
{
    public class QueueRunCommand
    {
        public string Repository { get; init; }
        public RunConfiguration RunConfiguration { get; init; }
    }
}