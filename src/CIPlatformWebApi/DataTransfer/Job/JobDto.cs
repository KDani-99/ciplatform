﻿using System;
using CIPlatform.Data.Configuration;

namespace CIPlatformWebApi.DataTransfer.Job
{
    public class JobDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartedDateTime { get; set; }
        public DateTime? FinishedDateTime { get; set; }
        public string JobContext { get; set; }
        public States State { get; set; }
        public int Steps { get; set; }
    }
}