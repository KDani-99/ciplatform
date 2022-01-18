using CodeManager.Data.Configuration;

namespace CodeManager.Data.Events
{
    public class QueueRunEvent
    {
        public RunConfiguration RunConfiguration { get; set; }
        public string ContextFilePath { get; set; } // github path
    }
}