using System.Collections.Generic;

namespace CodeManagerWebApi.DataTransfer
{
    public class StepFileDto
    {
        public IEnumerable<StepLogDto> StepLogs { get; set; }
    }
}