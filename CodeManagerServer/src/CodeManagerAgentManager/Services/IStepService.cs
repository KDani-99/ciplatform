using System.Threading.Tasks;

namespace CodeManagerAgentManager.Services
{
    public interface IStepService<T>
    {
        public Task ProcessStepResultAsync(T context, string connectionId);
    }
}