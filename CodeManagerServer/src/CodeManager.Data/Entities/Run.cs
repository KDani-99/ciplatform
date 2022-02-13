using System;
using System.Collections.Generic;
using CodeManager.Data.Configuration;

namespace CodeManager.Data.Entities
{
    public class Run : Entity
    {
        public string Repository { get; set; }
        public List<Job> Jobs { get; set; }
        public States State { get; set; } = States.NotRun;
        public DateTime? StartedDateTime { get; set; }
        public DateTime? FinishedDateTime { get; set; }
        public Project Project { get; set; }
        public int NumberOfCompletedSteps { get; set; }
        public int NumberOfSteps { get; set; }
    }
}