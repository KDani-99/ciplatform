using System;
using CodeManager.Data.Configuration;

namespace CodeManagerWebApi.DataTransfer
{
    public class StepDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public States State { get; set; }
        public DateTime StartedDateTime { get; set; }
        public DateTime FinishedDateTime { get; set; }
        public long ExecutionTime { get; set; }
    }
}