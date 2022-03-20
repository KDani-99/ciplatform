using System.Threading.Tasks;

namespace CIPlatformManager.Services.Steps
{
    public interface IStepService<T>
    {
        public Task ProcessStepResultAsync(T context, string connectionId);
    }
}