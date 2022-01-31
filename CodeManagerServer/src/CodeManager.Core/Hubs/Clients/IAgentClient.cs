using System.Threading.Tasks;
using CodeManager.Data.Events;

namespace CodeManager.Core.Hubs.Clients
{
    public interface IAgentClient
    {
        public Task QueueDockerJob(QueueJobCommand queueDockerJobCommand);
        public Task QueueWindowsJob(QueueJobCommand queueWindowsJobCommand);
        public Task QueueLinuxJob(QueueJobCommand queueWindowsJobCommand);
    }
}