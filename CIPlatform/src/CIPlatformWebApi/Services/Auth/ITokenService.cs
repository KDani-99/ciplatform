using System.Security.Claims;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;

namespace CIPlatformWebApi.Services.Auth
{
    public interface ITokenService<T>
    {
        public Task<T> CreateAccessTokenAsync(UserEntity user);
        public Task<T> CreateRefreshTokenAsync(UserEntity user);
        public Task InvalidateAccessTokenAsync(string username);
        public Task InvalidRefreshTokenAsync(string username);
        public Task<ClaimsPrincipal> VerifyRefreshTokenAsync(string token);
        public Task<ClaimsPrincipal> VerifyAccessTokenAsync(string token);
    }
}