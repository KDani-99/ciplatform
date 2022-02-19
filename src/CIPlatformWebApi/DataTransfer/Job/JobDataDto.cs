using System.Collections.Generic;
using CIPlatformWebApi.DataTransfer.Step;

namespace CIPlatformWebApi.DataTransfer.Job
{
    public class JobDataDto
    {
        public JobDto Job { get; set; }
        public IEnumerable<StepDto> Steps { get; set; }
    }
}