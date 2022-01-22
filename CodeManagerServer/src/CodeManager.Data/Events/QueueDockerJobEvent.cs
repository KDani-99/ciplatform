namespace CodeManager.Data.Events
{
    public class QueueDockerJobEvent : IQueueJobEvent
    {
        public string Token { get; set; }
    }
}