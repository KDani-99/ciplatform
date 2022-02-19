using System;
using System.Collections.Generic;
using CIPlatform.Data.Configuration;

namespace CIPlatform.Data.Entities
{
    public class JobEntity : Entity
    {
        public string Name { get; set; }
        public long Index { get; set; }
        public List<StepEntity> Steps { get; set; }
        public RunEntity Run { get; set; }
        public States State { get; set; } = States.NotRun;
        public string JsonContext { get; set; }
        public JobContext Context { get; set; }
        public DateTime? StartedDateTime { get; set; }
        public DateTime? FinishedDateTime { get; set; }
    }
}