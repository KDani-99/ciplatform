using System.Threading.Tasks;
using CIPlatform.Data.Events;

namespace CIPlatform.Core.Hubs.Clients
{
    public interface IAgentClient
    {
        public Task QueueDockerJob(QueueJobCommand queueDockerJobCommand);
        public Task QueueWindowsJob(QueueJobCommand queueWindowsJobCommand);
        public Task QueueLinuxJob(QueueJobCommand queueWindowsJobCommand);
    }
}