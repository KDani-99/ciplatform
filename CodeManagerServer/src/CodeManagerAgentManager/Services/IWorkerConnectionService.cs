using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;

namespace CodeManagerAgentManager.Services
{
    public interface IWorkerConnectionService
    {
        public Task<WorkerConnectionData> GetWorkerConnectionAsync(string connectionId);
        public Task AddWorkerConnectionOfTypeAsync(WorkerConnectionData workerConnectionData);
        public Task RemoveWorkerConnectionAsync(string connectionId);
        public Task UpdateWorkerConnectionAsync(WorkerConnectionData workerConnectionData);
        public Task<IEnumerable<string>> GetAvailableWorkerConnectionIdsOfTypeAsync(JobContext jobContext);
        public Task<IEnumerable<WorkerConnectionData>> GetAvailableWorkerConnectionsAsync();
    }
}