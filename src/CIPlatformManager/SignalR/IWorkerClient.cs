using System.Threading.Tasks;
using CIPlatform.Data.Events;

namespace CIPlatformManager.WebSocket
{
    public interface IWorkerClient
    {
        public Task QueueJob(QueueJobCommand queueDockerJobCommand);
    }
}