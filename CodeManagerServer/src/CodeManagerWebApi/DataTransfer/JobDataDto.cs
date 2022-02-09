using System.Collections.Generic;

namespace CodeManagerWebApi.DataTransfer
{
    public class JobDataDto
    {
        public JobDto Job { get; set; }
        public IEnumerable<StepDto> Steps { get; set; }
    }
}