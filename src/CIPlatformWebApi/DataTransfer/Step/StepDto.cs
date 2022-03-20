using System;
using CIPlatform.Data.Configuration;

namespace CIPlatformWebApi.DataTransfer.Step
{
    public class StepDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public States State { get; set; }
        public DateTime? StartedDateTime { get; set; }
        public DateTime? FinishedDateTime { get; set; }
    }
}