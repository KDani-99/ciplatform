using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeManagerAgentManager.Services
{
    public interface ITokenService<T>
    {
        public Task<T> CreateJobTokenAsync(long runId, long jobId);
        public Task<ClaimsPrincipal> VerifyJobTokenAsync(string token);

        public Task<T> CreateJobRequestTokenAsync(long runId, long jobId);
        
        public Task<ClaimsPrincipal> VerifyJobRequestTokenAsync(string token);
    }
}