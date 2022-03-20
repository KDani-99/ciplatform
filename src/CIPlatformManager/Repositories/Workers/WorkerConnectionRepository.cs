using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatformManager.Cache;
using Microsoft.OpenApi.Extensions;

namespace CIPlatformManager.Repositories.Workers
{
    public class WorkerConnectionRepository : IWorkerConnectionRepository
    {
        private const string ConnectionsHashKey = "connections";
        private readonly IRedisConnectionCache _cache;

        public WorkerConnectionRepository(IRedisConnectionCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<string> GetAsync(string connectionId)
        {
            var result = await _cache.Database.HashGetAsync(ConnectionsHashKey, connectionId);
            return result.ToString();
        }

        public Task<bool> AddAsync(string connectionId, string workerData)
        {
            // Associate jobContext name with connection in the database
            return _cache.Database.HashSetAsync(ConnectionsHashKey, connectionId, workerData);
        }

        public async Task<bool> AddToPoolAsync(JobContext jobContext, string connectionId)
        {
            return await _cache.Database.ListRightPushAsync(jobContext.GetDisplayName(), connectionId) != 0;
        }

        public Task<bool> RemoveAsync(string connectionId)
        {
            return _cache.Database.HashDeleteAsync(ConnectionsHashKey, connectionId);
        }

        public Task RemoveFromPoolAsync(JobContext jobContext, string connectionId)
        {
            return _cache.Database.ListRemoveAsync(jobContext.GetDisplayName(), connectionId);
        }

        public async Task<string> RemoveFromPoolAsync(JobContext jobContext)
        {
            var result = await _cache.Database.ListLeftPopAsync(jobContext.GetDisplayName());
            return result;
        }

        public Task<bool> UpdateAsync(string connectionId, string workerData)
        {
            return AddAsync(connectionId, workerData);
        }

        public async Task<IEnumerable<string>> GetAllAsync(JobContext jobContext)
        {
            var workerConnections = await _cache.Database.ListRangeAsync(jobContext.GetDisplayName());

            return workerConnections.Select(connection => connection.ToString());
        }
    }
}