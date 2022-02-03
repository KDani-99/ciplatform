using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeManagerAgentManager.Services
{
    public interface ITokenService<T>
    {
        public Task<T> CreateJobTokenAsync(long runId, long jobId);
        public Task<ClaimsPrincipal> VerifyJobTokenAsync(string token);
    }
}