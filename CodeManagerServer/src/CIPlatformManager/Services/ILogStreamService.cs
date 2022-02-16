using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatform.Data.Events;

namespace CIPlatformManager.Services
{
    public interface ILogStreamService
    {
        public Task ProcessStreamAsync(ChannelReader<string> stream, long runId, long jobId, int step);
    }
}