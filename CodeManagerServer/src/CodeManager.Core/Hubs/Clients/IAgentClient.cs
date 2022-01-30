using System.Threading.Tasks;
using CodeManager.Data.Events;

namespace CodeManager.Core.Hubs.Clients
{
    public interface IAgentClient
    {
        public Task QueueDockerJob(QueueJobEvent queueDockerJobEvent);
        public Task QueueWindowsJob(QueueJobEvent queueWindowsJobEvent);
        public Task QueueLinuxJob(QueueJobEvent queueWindowsJobEvent);
    }
}