using System.Threading.Tasks;

namespace CodeManagerAgentManager.Services
{
    public interface IRunService<T>
        where T : new()

    {
        public Task<long> QueueAsync(T cmd);
    }
}