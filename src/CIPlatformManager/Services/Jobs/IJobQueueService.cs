using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using CIPlatformManager.Entities;

namespace CIPlatformManager.Services.Jobs
{
    public interface IJobQueueService
    {
        public Task QueueAsync(CachedJobDetails jobDetails, JobContext jobContext);
        public Task AddFrontAsync(CachedJobDetails jobDetails, JobContext jobContext);
        public Task<CachedJobDetails> DequeueAsync(JobContext jobContext);
    }
}