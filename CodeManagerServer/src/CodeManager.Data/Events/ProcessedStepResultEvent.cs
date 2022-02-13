using System;
using CodeManager.Data.Configuration;

namespace CodeManager.Data.Entities
{
    public class ProcessedStepResult
    {
        public long JobId { get; set; }
        public long StepId { get; set; }
        public States State { get; set; }
        public DateTime DateTime { get; set; }
    }
}