using System;
using System.Threading.Tasks;

namespace CodeManagerWebApi.Repositories
{
    public interface ITokenRepository
    {
        public Task<string> GetAccessTokenAsync(string username);
        public Task<string> GetRefreshTokenAsync(string username);
        public Task<bool> AddAccessTokenAsync(string username, string accessToken, TimeSpan expiration);
        public Task<bool> AddRefreshTokenAsync(string username, string accessToken, TimeSpan expiration);
        public Task<bool> DeleteAccessTokenAsync(string username);
        public Task<bool> DeleteRefreshTokenAsync(string username);
    }
}