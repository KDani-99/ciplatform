using System.Collections.Generic;

namespace CodeManagerWebApi.DataTransfer
{
    public class RunDataDto
    {
        public RunDto Run { get; set; }
        public IEnumerable<JobDto> Jobs { get; set; }
    }
}