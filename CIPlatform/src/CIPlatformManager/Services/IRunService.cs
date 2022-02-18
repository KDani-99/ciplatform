using System.Threading.Tasks;
using CIPlatform.Data.Commands;

namespace CIPlatformManager.Services
{
    public interface IRunService
    {
        public Task QueueAsync(QueueRunCommand cmd);
    }
}