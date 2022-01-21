namespace CodeManager.Data.Events
{
    public class VerifiedStepLogEvent
    {
        public long RunId { get; init; }
        public long JobId { get; init; }
        public string Log { get; init; }
        public long Line { get; init; }
        public int StepIndex { get; init; }
    }
}