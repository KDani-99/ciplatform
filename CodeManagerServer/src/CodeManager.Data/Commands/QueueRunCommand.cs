using CodeManager.Data.Configuration;

namespace CodeManager.Data.Commands
{
    public class QueueRunCommand
    {
        public string Repository { get; set; }
        public RunConfiguration RunConfiguration { get; set; }
        public long ProjectId { get; set; }
    }
}