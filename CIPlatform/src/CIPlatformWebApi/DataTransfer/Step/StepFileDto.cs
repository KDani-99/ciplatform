using System.Collections.Generic;

namespace CIPlatformWebApi.DataTransfer
{
    public class StepFileDto
    {
        public IEnumerable<StepLogDto> StepLogs { get; set; }
    }
}