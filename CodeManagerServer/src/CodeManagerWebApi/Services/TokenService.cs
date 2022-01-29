using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CodeManagerWebApi.Services
{
    public class TokenService : ITokenService<JwtSecurityToken>
    {
        private readonly JwtConfiguration _jwtConfiguration;

        public TokenService(IOptions<JwtConfiguration> configuration)
        {
            _jwtConfiguration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Task<JwtSecurityToken> CreateAccessToken(User user)
        {
            var claims = new []
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            return Task.FromResult(GenerateToken(claims, _jwtConfiguration.LifeTimeMinutes,
                SecurityAlgorithms.HmacSha256Signature));
        }

        public Task<JwtSecurityToken> CreateRefreshToken(User user)
        {
            var claims = new []
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            return Task.FromResult(GenerateToken(claims, _jwtConfiguration.LifeTimeMinutes,
                SecurityAlgorithms.HmacSha512Signature)); // should be a stronger signing algorithm
        }

        public Task CreateVerificationToken()
        {
            throw new System.NotImplementedException();
        }

        public Task<ClaimsPrincipal> VerifyAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler {MapInboundClaims = false};
            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Secret)),
            };

            return Task.FromResult(tokenHandler.ValidateToken(token, validationParameters, out _));
        }

        private JwtSecurityToken GenerateToken(IEnumerable<Claim> claims, int lifetimeInMinutes, string algorithm)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretBytes = Encoding.UTF8.GetBytes(_jwtConfiguration.Secret);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(lifetimeInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretBytes), algorithm),
            };

           /* var securityToken = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(securityToken);*/
           
           return tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        }
    }
}