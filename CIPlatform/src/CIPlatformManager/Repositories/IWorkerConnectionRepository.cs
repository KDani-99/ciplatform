﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;

namespace CIPlatformManager.Repositories
{
    public interface IWorkerConnectionRepository
    {
        public Task<string> GetAsync(string connectionId);
        public Task<bool> AddAsync(string connectionId, string workerData);
        public Task<bool> AddToPoolAsync(JobContext jobContext, string connectionId);
        public Task<bool> RemoveAsync(string connectionId); // TODO: removes from data hash

        public Task
            RemoveFromPoolAsync(JobContext jobContext,
                                string connectionId); // TODO: removes only from the pool = makes it unavailable

        public Task<string> RemoveFromPoolAsync(JobContext jobContext);

        public Task<bool> UpdateAsync(string connectionId, string workerData);
        public Task<IEnumerable<string>> GetAllAsync(JobContext jobContext);
    }
}