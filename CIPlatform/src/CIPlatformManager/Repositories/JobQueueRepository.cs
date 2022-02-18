using System;
using System.Threading.Tasks;
using CIPlatform.Data.Events;
using CIPlatformManager.Cache;
using MassTransit.JobService;
using Microsoft.OpenApi.Extensions;
using JobContext = CIPlatform.Data.Configuration.JobContext;

namespace CIPlatformManager.Repositories
{
    public class JobQueueRepository : IJobQueueRepository
    {
        private readonly IRedisJobQueueCache _cache;

        public JobQueueRepository(IRedisJobQueueCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public Task AddAsync(string jobData, JobContext jobContext, bool front)
        {
            return front ? _cache.Database.ListLeftPushAsync(jobContext.GetDisplayName(), jobData) : _cache.Database.ListRightPushAsync(jobContext.GetDisplayName(), jobData);
        }

        public async Task<string> RemoveAsync(JobContext jobContext)
        {
            var value = await _cache.Database.ListLeftPopAsync(jobContext.GetDisplayName());
            return value;
        }

    }
}