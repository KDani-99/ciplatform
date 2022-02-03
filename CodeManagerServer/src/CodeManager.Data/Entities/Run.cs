﻿using System;
using System.Collections.Generic;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;

namespace CodeManager.Data.Entities.CI
{
    public class Run : Entity
    {
        public string Repository { get; set; }
        public List<Job> Jobs { get; set; }
        public States State { get; set; } = States.NotRun;
        public DateTime StartedDateTime { get; set; }
        public DateTime FinishedDateTime { get; set; }
        
        public Project Project { get; set; }
    }
}