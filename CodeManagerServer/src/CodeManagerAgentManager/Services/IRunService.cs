using System.Threading.Tasks;
using CodeManager.Data.Commands;

namespace CodeManagerAgentManager.Services
{
    public interface IRunService
    {
        public Task<long> QueueAsync(QueueRunCommand cmd);
    }
}