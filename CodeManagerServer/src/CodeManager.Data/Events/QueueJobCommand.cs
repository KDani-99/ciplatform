using CodeManager.Data.Configuration;

namespace CodeManager.Data.Events
{
    public class QueueJobCommand
    {
        public string Token { get; set; }
        public JobConfiguration JobConfiguration { get; set; }
        public string Repository { get; set; }
    }
}