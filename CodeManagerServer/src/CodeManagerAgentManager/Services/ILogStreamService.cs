using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeManagerAgentManager.Services
{
    public interface ILogStreamService
    {
        public Task ProcessStreamAsync(IAsyncEnumerable<string> stream, long runId, long jobId, int step);
    }
}