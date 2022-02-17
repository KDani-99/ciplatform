using System.Threading.Tasks;
using CIPlatform.Data.Events;

namespace IPlatformManager.WebSocket
{
    public interface IWorkerClient
    {
        public Task QueueDockerJob(QueueJobCommand queueDockerJobCommand);
        public Task QueueWindowsJob(QueueJobCommand queueWindowsJobCommand);
        public Task QueueLinuxJob(QueueJobCommand queueWindowsJobCommand);
    }
}