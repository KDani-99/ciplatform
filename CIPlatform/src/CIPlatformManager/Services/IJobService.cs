using System.Threading.Tasks;
using CIPlatform.Data.Events;
using CIPlatformManager.Entities;

namespace CIPlatformManager.Services
{
    public interface IJobService
    {
        public Task QueueJobAsync(CachedJobDetails jobDetails, string connectionId);
    }
}