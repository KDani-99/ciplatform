using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatform.Data.Events;

namespace CIPlatformManager.Services.Logs
{
    public interface ILogStreamService
    {
        public Task ProcessStreamAsync(ChannelReader<string> stream, long runId, long jobId, int step);
    }
}