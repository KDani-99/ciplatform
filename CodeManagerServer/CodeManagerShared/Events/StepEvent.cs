namespace CodeManagerShared.Events
{
    public abstract class StepEvent
    {
        public string AgentId { get; set; }
        public int StepIndex { get; set; }
    }
}