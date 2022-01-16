using System;
using System.Collections.Generic;
using CodeManager.Data.Entities;

namespace CodeManager.Data.Entities.CI
{
    public class Run : Entity
    {
        public string ContextFilePath { get; set; } // git path
        public List<Job> Jobs { get; set; }
        public DateTime StartedDateTime { get; set; }
        public DateTime FinishedDateTime { get; set; }
    }
}