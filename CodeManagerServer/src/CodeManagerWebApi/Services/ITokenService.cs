using System.Security.Claims;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Services
{
    public interface ITokenService<T>
    {
        public Task<T> CreateAccessToken(User user);
        public Task<T> CreateRefreshToken(User user);
        public Task CreateVerificationToken();

        public Task<ClaimsPrincipal> VerifyAccessToken(string token);
    }
}