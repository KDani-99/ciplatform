using System.Threading.Tasks;
using CIPlatform.Data.Commands;

namespace CIPlatformManager.Services.Runs
{
    public interface IRunService
    {
        public Task QueueAsync(QueueRunCommand cmd);
    }
}