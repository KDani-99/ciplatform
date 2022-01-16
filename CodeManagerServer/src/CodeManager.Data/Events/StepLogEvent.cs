namespace CodeManager.Data.Events
{
    public class StepLogEvent : ISecureMessage
    {
        public string Token { get; set; }
        public string Log { get; set; }
        public int StepIndex { get; set; }
    }
}