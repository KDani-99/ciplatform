﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.JsonWebTokens;
using CodeManagerAgentManager.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CodeManagerAgentManager.Services
{
    public class TokenService : ITokenService<JwtSecurityToken>
    {
        private readonly TokenServiceConfiguration _tokenServiceConfiguration;

        public TokenService(IOptions<TokenServiceConfiguration> tokenServiceConfiguration)
        {
            _tokenServiceConfiguration = tokenServiceConfiguration.Value ??
                                         throw new ArgumentNullException(nameof(tokenServiceConfiguration));
        }

        public Task<JwtSecurityToken> CreateJobTokenAsync(long runId, long jobId)
        {
            return CreateJobTokenAsync(runId, jobId, _tokenServiceConfiguration.JobTokenConfiguration);
        }

        public Task<ClaimsPrincipal> VerifyJobTokenAsync(string token)
        {
            return VerifyJobTokenAsync(token, _tokenServiceConfiguration.JobTokenConfiguration.Secret);
        }

        public Task<JwtSecurityToken> CreateJobRequestTokenAsync(long runId, long jobId)
        {
            return CreateJobTokenAsync(runId, jobId, _tokenServiceConfiguration.JobRequestTokenConfiguration);
        }

        public Task<ClaimsPrincipal> VerifyJobRequestTokenAsync(string token)
        {
            return VerifyJobTokenAsync(token, _tokenServiceConfiguration.JobRequestTokenConfiguration.Secret);
        }

        private static Task<JwtSecurityToken> CreateJobTokenAsync(long runId, long jobId,
            TokenConfiguration tokenConfiguration)
        {
            var claims = new[]
            {
                new Claim(CustomJwtRegisteredClaimNames.RunId, runId.ToString()),
                new Claim(CustomJwtRegisteredClaimNames.JobId, jobId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            return Task.FromResult(GenerateToken(claims, tokenConfiguration));
        }

        private static Task<ClaimsPrincipal> VerifyJobTokenAsync(string token, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler {MapInboundClaims = false};
            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };

            return Task.FromResult(tokenHandler.ValidateToken(token, validationParameters, out _));
        }

        private static JwtSecurityToken GenerateToken(IEnumerable<Claim> claims, TokenConfiguration tokenConfiguration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretBytes = Encoding.UTF8.GetBytes(tokenConfiguration.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(tokenConfiguration.LifeTimeMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        }
    }
}