using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;

namespace CIPlatformManager.Repositories.Jobs
{
    public interface IJobQueueRepository
    {
        public Task AddAsync(string jobData, JobContext jobContext, bool front);
        public Task<string> RemoveAsync(JobContext jobContext);
    }
}