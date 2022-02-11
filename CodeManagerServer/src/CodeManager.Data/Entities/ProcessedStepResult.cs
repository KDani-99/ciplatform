using CodeManager.Data.Configuration;

namespace CodeManager.Data.Entities
{
    public class ProcessedStepResult
    {
        public States State { get; set; }
        public long StepIndex { get; set; }
        public long RunId { get; set; }
        public long JobId { get; set; }
        public long ProjectId { get; set; }
    }
}