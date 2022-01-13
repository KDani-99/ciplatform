using CodeManagerShared.Entities;

namespace CodeManagerShared.Events
{
    public class StepResultEvent : StepEvent
    {
        public States State { get; set; }
    }
}