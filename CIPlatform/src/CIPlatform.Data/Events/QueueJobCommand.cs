using CIPlatform.Data.Configuration;

namespace CIPlatform.Data.Events
{
    public class QueueJobCommand
    {
        public string Token { get; set; }
        public JobConfiguration JobConfiguration { get; set; }
        public string Repository { get; set; }
    }
}