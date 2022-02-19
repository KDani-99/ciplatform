using System;
using System.Collections.Generic;
using CIPlatform.Data.Configuration;

namespace CIPlatform.Data.Entities
{
    public class RunEntity : Entity
    {
        public string Repository { get; set; }
        public List<JobEntity> Jobs { get; set; }
        public States State { get; set; } = States.NotRun;
        public DateTime? StartedDateTime { get; set; }
        public DateTime? FinishedDateTime { get; set; }
        public ProjectEntity Project { get; set; }
        public int NumberOfCompletedSteps { get; set; }
        public int NumberOfSteps { get; set; }
    }
}