namespace CodeManager.Data.Events
{
    public class RunQueuedEvent
    {
        public long RunId { get; set; }
        public long ProjectId { get; set; }
    }
}