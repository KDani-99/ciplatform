using System.Collections.Generic;

namespace CIPlatformWebApi.DataTransfer.Step
{
    public class StepFileDto
    {
        public IEnumerable<StepLogDto> StepLogs { get; set; }
    }
}