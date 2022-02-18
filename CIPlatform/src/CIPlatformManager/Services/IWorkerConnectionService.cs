using System.Collections.Generic;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;

namespace CIPlatformManager.Services
{
    public interface IWorkerConnectionService
    {
        public Task<WorkerConnectionDataEntity> GetWorkerConnectionAsync(string connectionId);
        public Task AddWorkerConnectionOfTypeAsync(WorkerConnectionDataEntity workerConnectionData);
        public Task RemoveWorkerConnectionAsync(string connectionId);
        public Task UpdateWorkerConnectionAsync(WorkerConnectionDataEntity workerConnectionData);
        public Task<IEnumerable<string>> GetAvailableWorkerConnectionIdsOfTypeAsync(JobContext jobContext);
        public Task<string> DequeueAvailableWorkerConnectionOfTypeAsync(JobContext jobContext);
        public Task QueueWorkerConnectionOfTypeAsync(JobContext jobContext, string connectionId);
        public Task KeepWorkerConnectionAsync(string connectionId);
        public Task MarkWorkerConnectionAsAvailableAsync(string connectionId);
    }
}