using CodeManager.Data.Configuration;

namespace CodeManager.Data.Commands
{
    public class SuccessfulQueueRunCommandResponse
    {
        public long RunId { get; init; }
        public RunConfiguration RunConfiguration { get; set; }
    }
}