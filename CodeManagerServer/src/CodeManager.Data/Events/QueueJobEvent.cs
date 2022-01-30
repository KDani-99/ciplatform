using CodeManager.Data.Configuration;

namespace CodeManager.Data.Events
{
    public class QueueJobEvent
    {
        public string Token { get; init; }
        public JobConfiguration JobConfiguration { get; init; }
        public string Repository { get; init; }
    }
}