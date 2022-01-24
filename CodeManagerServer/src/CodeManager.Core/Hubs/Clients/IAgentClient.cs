using System.Threading.Tasks;
using CodeManager.Data.Events;

namespace CodeManager.Core.Hubs.Clients
{
    public interface IAgentClient
    {
        public Task<bool> QueueDockerJobEvent(QueueDockerJobEvent queueDockerJobEvent);
        public Task<bool> QueueWindowsJobEvent(QueueWindowsJobEvent queueWindowsJobEvent);
        public Task<bool> QueueLinuxJobEvent(QueueWindowsJobEvent queueWindowsJobEvent);
    }
}