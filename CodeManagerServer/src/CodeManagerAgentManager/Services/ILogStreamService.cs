using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeManagerAgentManager.Services
{
    public interface ILogStreamService
    {
        public Task WriteStreamAsync(IAsyncEnumerable<string> stream, long runId, long jobId, int step);
    }
}