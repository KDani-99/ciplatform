using System;
using CodeManager.Data.Configuration;

namespace CodeManagerWebApi.DataTransfer
{
    public class RunDto
    {
        public long Id { get; set; }
        public DateTime StartedDateTime { get; set; }
        public DateTime FinishedDateTime { get; set; }
        public long ExecutionTime { get; set; }
        public States State { get; set; }
        public int Jobs { get; set; }
    }
}