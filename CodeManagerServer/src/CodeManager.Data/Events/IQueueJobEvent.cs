namespace CodeManager.Data.Events
{
    public interface IQueueJobEvent
    {
        public string Token { get; set; }
    }
}