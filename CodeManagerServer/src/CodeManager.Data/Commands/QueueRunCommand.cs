using CodeManager.Data.Configuration;

namespace CodeManager.Data.Commands
{
    public class QueueRunCommand
    {
        public RunConfiguration RunConfiguration { get; set; }
        public string ContextFilePath { get; set; } // github path
        public string RunId { get; set; }
    }
}