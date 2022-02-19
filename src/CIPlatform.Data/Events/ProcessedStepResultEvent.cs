using System;
using CIPlatform.Data.Configuration;

namespace CIPlatform.Data.Entities
{
    public class ProcessedStepResultEvent
    {
        public long JobId { get; set; }
        public long StepId { get; set; }
        public States State { get; set; }
        public DateTime? DateTime { get; set; }
    }
}