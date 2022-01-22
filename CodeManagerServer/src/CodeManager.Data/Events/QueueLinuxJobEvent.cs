namespace CodeManager.Data.Events
{
    public class QueueLinuxJobEvent : IQueueJobEvent
    {
        public string Token { get; set; }
    }
}