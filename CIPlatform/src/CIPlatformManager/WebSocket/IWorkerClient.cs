using System.Threading.Tasks;
using CIPlatform.Data.Events;

namespace IPlatformManager.WebSocket
{
    public interface IWorkerClient
    {
        public Task QueueJob(QueueJobCommand queueDockerJobCommand);
    }
}