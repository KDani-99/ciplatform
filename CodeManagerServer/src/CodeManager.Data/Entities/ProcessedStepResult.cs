using CodeManager.Data.Configuration;

namespace CodeManager.Data.Entities
{
    public class ProcessedStepResult
    {
        public States State { get; set; }
        public long StepId { get; set; }
    }
}