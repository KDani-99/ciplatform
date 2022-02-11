using System;
using System.Collections.Generic;
using CodeManager.Data.Configuration;

namespace CodeManager.Data.Entities
{
    public class Job : Entity
    {
        public string Name { get; set; }
        public List<Step> Steps { get; set; }
        public Run Run { get; set; }
        public States State { get; set; } = States.NotRun;
        public string JsonContext { get; set; }
        public JobContext Context { get; set; }
        public DateTime StartedDateTime { get; set; }
        public DateTime FinishedDateTime { get; set; }
    }
}