using System.Threading.Tasks;
using CodeManager.Data.Events;

namespace CodeManagerAgentManager.Services
{
    public interface IJobService
    {
        public Task<IQueueJobEvent> ProcessJobRequestTokenAsync(string token);
    }
}