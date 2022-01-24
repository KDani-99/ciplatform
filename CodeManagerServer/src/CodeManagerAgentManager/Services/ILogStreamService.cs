using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeManagerAgentManager.Services
{
    public interface ILogStreamService
    {
        public Task WriteStreamAsync(int runId, int jobId, int step, IAsyncEnumerable<string> stream);
    }
}