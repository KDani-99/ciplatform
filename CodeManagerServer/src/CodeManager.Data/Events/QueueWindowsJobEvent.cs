namespace CodeManager.Data.Events
{
    public class QueueWindowsJobEvent : IQueueJobEvent
    {
        public string Token { get; set; }
    }
}