using System.Collections.Generic;

namespace CIPlatformWebApi.DataTransfer
{
    public class RunDataDto
    {
        public RunDto Run { get; set; }
        public IEnumerable<JobDto> Jobs { get; set; }
    }
}