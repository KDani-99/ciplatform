using System.Threading.Tasks;
using CodeManager.Data.Events;

namespace CodeManager.Core.Hubs.Clients
{
    public interface IAgentClient
    {
        public Task QueueDockerJobEvent(QueueDockerJobEvent queueDockerJobEvent);
        public Task QueueWindowsJobEvent(QueueWindowsJobEvent queueWindowsJobEvent);
        public Task QueueLinuxJobEvent(QueueWindowsJobEvent queueWindowsJobEvent);
    }
}