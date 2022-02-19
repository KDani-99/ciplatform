using System.Collections.Generic;
using CIPlatformWebApi.DataTransfer.Job;

namespace CIPlatformWebApi.DataTransfer.Run
{
    public class RunDataDto
    {
        public RunDto Run { get; set; }
        public IEnumerable<JobDto> Jobs { get; set; }
    }
}