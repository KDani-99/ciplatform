using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CodeManagerAgentManager.Services
{
    public interface ILogStreamService
    {
        public Task ProcessStreamAsync(ChannelReader<string> stream, long runId, long jobId, int step);
    }
}