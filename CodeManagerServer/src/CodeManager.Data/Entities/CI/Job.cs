﻿using System;
using System.Collections.Generic;
using CodeManager.Data.Entities;

namespace CodeManager.Data.Entities.CI
{
    public class Job : Entity
    {
        public string Name { get; set; }
        public List<Step> Steps { get; set; }
        public Run Run { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime FinishedDateTime { get; set; }
    }
}