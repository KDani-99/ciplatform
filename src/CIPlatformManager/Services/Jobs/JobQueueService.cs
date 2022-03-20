using System;
using System.Text.Json;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using CIPlatformManager.Entities;
using CIPlatformManager.Repositories;
using CIPlatformManager.Repositories.Jobs;

namespace CIPlatformManager.Services.Jobs
{
    public class JobQueueService : IJobQueueService
    {
        private readonly IJobQueueRepository _jobQueueRepository;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public JobQueueService(IJobQueueRepository jobQueueRepository,  JsonSerializerOptions jsonSerializerOptions)
        {
            _jobQueueRepository = jobQueueRepository ?? throw new ArgumentNullException(nameof(jobQueueRepository));
            _jsonSerializerOptions =
                jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
        }
        
        public Task QueueAsync(CachedJobDetails jobDetails, JobContext jobContext)
        {
            return _jobQueueRepository.AddAsync(JsonSerializer.Serialize(jobDetails, _jsonSerializerOptions), jobContext, false);
        }

        public Task AddFrontAsync(CachedJobDetails jobDetails, JobContext jobContext)
        {
            return _jobQueueRepository.AddAsync(JsonSerializer.Serialize(jobDetails, _jsonSerializerOptions), jobContext, true);
        }

        public async Task<CachedJobDetails> DequeueAsync(JobContext jobContext)
        {
            var job = await _jobQueueRepository.RemoveAsync(jobContext);

            return job is null ? null : JsonSerializer.Deserialize<CachedJobDetails>(job);
        }
    }
}