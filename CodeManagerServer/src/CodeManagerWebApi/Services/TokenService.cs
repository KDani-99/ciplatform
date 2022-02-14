using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.Configuration;
using CodeManagerWebApi.Exceptions;
using CodeManagerWebApi.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CodeManagerWebApi.Services
{
    public class TokenService : ITokenService<JwtSecurityToken>
    {
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository, IOptions<JwtConfiguration> configuration)
        {
            _tokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
            _jwtConfiguration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<JwtSecurityToken> CreateAccessTokenAsync(User user)
        {
            var jti = Guid.NewGuid().ToString();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            await _tokenRepository.AddAccessTokenAsync(user.Username, jti,
                                                       TimeSpan.FromSeconds(_jwtConfiguration.AccessTokenLifeTime));

            return GenerateToken(claims, _jwtConfiguration.AccessTokenLifeTime,
                                 SecurityAlgorithms.HmacSha256Signature);
        }

        public async Task<JwtSecurityToken> CreateRefreshTokenAsync(User user)
        {
            var jti = Guid.NewGuid().ToString();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            await _tokenRepository.AddRefreshTokenAsync(user.Username, jti,
                                                        TimeSpan.FromSeconds(_jwtConfiguration.RefreshTokenLifeTime));

            return GenerateToken(claims, _jwtConfiguration.RefreshTokenLifeTime,
                                 SecurityAlgorithms.HmacSha512Signature); // should be a stronger signing algorithm
        }

        public Task InvalidateAccessTokenAsync(string username)
        {
            return _tokenRepository.DeleteAccessTokenAsync(username);
        }

        public Task InvalidRefreshTokenAsync(string username)
        {
            return _tokenRepository.DeleteRefreshTokenAsync(username);
        }

        public async Task<ClaimsPrincipal> VerifyRefreshTokenAsync(string token)
        {
            var claimsPrincipal = await VerifyTokenAsync(token);
            var jti = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var username = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)
                                          ?.Value;
            var exp = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (jti == default || username == default || exp == default)
            {
                throw new UnauthorizedAccessWebException("Invalid refresh token provided.");
            }

            ValidateTokenLifetime(long.Parse(exp));

            var storedJti = await _tokenRepository.GetRefreshTokenAsync(username);

            if (jti != storedJti)
            {
                throw new UnauthorizedAccessWebException("Invalid refresh token provided.");
            }

            return claimsPrincipal;
        }

        public async Task<ClaimsPrincipal> VerifyAccessTokenAsync(string token)
        {
            var claimsPrincipal = await VerifyTokenAsync(token);
            
            var jti = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var username = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)
                                          ?.Value;
            var exp = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp)?.Value;
            
            if (jti == default || username == default || exp == default)
            {
                throw new UnauthorizedAccessWebException("Invalid refresh token provided.");
            }

            ValidateTokenLifetime(long.Parse(exp));

            var storedJti = await _tokenRepository.GetAccessTokenAsync(username);

            if (jti != storedJti)
            {
                throw new UnauthorizedAccessWebException("Invalid access token provided.");
            }

            return claimsPrincipal;
        }

        private Task<ClaimsPrincipal> VerifyTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Secret))
            };

            return Task.FromResult(tokenHandler.ValidateToken(token, validationParameters, out _));
        }

        private JwtSecurityToken GenerateToken(IEnumerable<Claim> claims, int lifeTime, string algorithm)
        {
            /*  var tokenHandler = new JwtSecurityTokenHandler();
              var secretBytes = Encoding.UTF8.GetBytes(_jwtConfiguration.Secret);
              
              var tokenDescriptor = new SecurityTokenDescriptor
              {
                  Subject = new ClaimsIdentity(claims),
                  Expires = DateTime.Now.AddSeconds(1),
                  SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretBytes), algorithm),
              };
  
             return tokenHandler.CreateJwtSecurityToken(tokenDescriptor);*/
            var secretBytes = Encoding.UTF8.GetBytes(_jwtConfiguration.Secret);

            var jwtHeader = new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(secretBytes), algorithm));

            var jwtPayload = new JwtPayload();
            jwtPayload.AddClaims(claims);

            jwtPayload.Add(JwtRegisteredClaimNames.Exp,
                           (int) (DateTime.Now.AddSeconds(lifeTime) - DateTime.UnixEpoch).TotalSeconds);

            return new JwtSecurityToken(jwtHeader, jwtPayload);
        }

        private void ValidateTokenLifetime(double expires)
        {
            if ((DateTime.UnixEpoch.AddSeconds(expires) - DateTime.Now).TotalSeconds <= 0)
            {
                throw new SecurityTokenExpiredException();
            }
        }
    }
}