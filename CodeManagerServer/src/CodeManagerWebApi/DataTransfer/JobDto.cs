using System;
using System.Collections;
using System.Collections.Generic;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities.CI;

namespace CodeManagerWebApi.DataTransfer
{
    public class JobDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartedDateTime { get; set; }
        public DateTime FinishedDateTime { get; set; }
        public long ExecutionTime { get; set; }
        public string JobContext { get; set; }
        public States State { get; set; }
        public int Steps { get; set; }
    }
}