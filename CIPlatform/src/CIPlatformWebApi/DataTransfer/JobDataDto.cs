using System.Collections.Generic;

namespace CIPlatformWebApi.DataTransfer
{
    public class JobDataDto
    {
        public JobDto Job { get; set; }
        public IEnumerable<StepDto> Steps { get; set; }
    }
}