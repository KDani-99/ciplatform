using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;

namespace CodeManagerAgentManager.Repositories
{
    public interface IWorkerConnectionRepository
    {
        public Task<string> GetAsync(string connectionId);
        public Task<bool> AddAsync(string connectionId, string workerData);
        public Task<bool> AddToPoolAsync(JobContext jobContext, string connectionId);
        public Task<bool> RemoveAsync(string connectionId); // TODO: removes from data hash
        public Task<bool> RemoveFromPoolAsync(JobContext jobContext, string connectionId); // TODO: removes only from the pool = makes it unavailable
        public Task<bool> UpdateAsync(string connectionId, string workerData);
        public Task<IEnumerable<string>> GetAllAsync(JobContext jobContext);
    }
}