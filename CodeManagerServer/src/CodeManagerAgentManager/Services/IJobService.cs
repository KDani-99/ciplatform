using System.Threading.Tasks;

namespace CodeManagerAgentManager.Services
{
    public interface IJobService<T>
        where T : new()
    {
        public Task<T> ProcessJobRequestTokenAsync(string token);
    }
}