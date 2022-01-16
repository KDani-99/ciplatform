namespace CodeManager.Core.Hubs.Messages
{
    public class LogMessage
    {
        // JobId, RunId will be handled by the manager
        public string StepId { get; set; }
        public string Log { get; set; }
    }
}