using System;
using System.Threading.Tasks;
using CodeManagerWebApi.Cache;

namespace CodeManagerWebApi.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ITokenCache _tokenCache;
        private const string RefreshTokensKey = "refresh_token";
        private const string AccessTokensKey = "access_token";
        
        public TokenRepository(ITokenCache tokenCache)
        {
            _tokenCache = tokenCache ?? throw new ArgumentNullException(nameof(tokenCache));
        }
        
        public async Task<string> GetAccessTokenAsync(string username)
        {
            var token = await _tokenCache.Database.StringGetAsync(GetAccessTokenKey(username));
            return token;
        }

        public async Task<string> GetRefreshTokenAsync(string username)
        {
            var token = await _tokenCache.Database.StringGetAsync(GetRefreshTokenKey(username));
            return token;
        }

        public Task<bool> AddAccessTokenAsync(string username, string accessToken, TimeSpan expiration)
        {
            return _tokenCache.Database.StringSetAsync(GetAccessTokenKey(username), accessToken, expiration);
        }

        public Task<bool> AddRefreshTokenAsync(string username, string refreshToken, TimeSpan expiration)
        {
            return _tokenCache.Database.StringSetAsync(GetRefreshTokenKey(username), refreshToken, expiration);
        }

        public Task<bool> DeleteAccessTokenAsync(string username)
        {
            return _tokenCache.Database.KeyDeleteAsync(GetAccessTokenKey(username));
        }

        public Task<bool> DeleteRefreshTokenAsync(string username)
        {
            return _tokenCache.Database.KeyDeleteAsync(GetRefreshTokenKey(username));
        }

        private static string GetRefreshTokenKey(string username)
        {
            return $"{RefreshTokensKey}_{username}";
        }
        private static string GetAccessTokenKey(string username)
        {
            return $"{AccessTokensKey}_{username}";
        }
    }
}