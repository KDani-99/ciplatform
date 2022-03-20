using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Extensions;
using CIPlatformWebApi.Configuration;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Repositories;
using CIPlatformWebApi.Services.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.Services
{
    public class TokenServiceTests
    {
        [Test]
        public async Task CreateAccessTokenAsync_ValidUser_ShouldCreateToken()
        {
            // Arrange
            var userEntity = new UserEntity
            {
                Username = "test",
                Roles = new[] { Roles.Admin }
            };
            var tokenRepository = new Mock<ITokenRepository>();
            var options = Options.Create(new JwtConfiguration
            {
                Secret = "69102919d888ae3439bd7b1476c0e28d3456b654875c857e9f868d7619798767"
            });
            
            var tokenService = new TokenService(tokenRepository.Object, options);

            var expectedClaimKeys = new[]
            {
                JwtRegisteredClaimNames.Sub,
                JwtRegisteredClaimNames.Jti,
                JwtRegisteredClaimNames.Exp
            };

            // Act
            var token = await tokenService.CreateAccessTokenAsync(userEntity);

            // Assert
            Assert.NotNull(token);
            Assert.AreEqual(expectedClaimKeys,token.Claims.Select(claim => claim.Type).ToList());
        }
        
        [Test]
        public async Task CreateRefreshTokenAsync_ValidUser_ShouldCreateToken()
        {
            // Arrange
            var userEntity = new UserEntity
            {
                Username = "test",
                Roles = new[] { Roles.Admin }
            };
            var tokenRepository = new Mock<ITokenRepository>();
            var options = Options.Create(new JwtConfiguration
            {
                Secret = "69102919d888ae3439bd7b1476c0e28d3456b654875c857e9f868d7619798767"
            });
            
            var tokenService = new TokenService(tokenRepository.Object, options);

            var expectedClaimKeys = new[]
            {
                JwtRegisteredClaimNames.Sub,
                JwtRegisteredClaimNames.Jti,
                JwtRegisteredClaimNames.Exp
            };

            // Act
            var token = await tokenService.CreateRefreshTokenAsync(userEntity);

            // Assert
            Assert.NotNull(token);
            Assert.AreEqual(expectedClaimKeys,token.Claims.Select(claim => claim.Type).ToList());
        }
        
        [Test]
        public async Task VerifyAccessTokenAsync_ExpiredToken_ShouldThrowSecurityTokenExpiredException()
        {
            // Arrange
            var userEntity = new UserEntity
            {
                Username = "test",
                Roles = new[] { Roles.Admin }
            };
            var tokenRepository = new Mock<ITokenRepository>();
            var options = Options.Create(new JwtConfiguration
            {
                Secret = "69102919d888ae3439bd7b1476c0e28d3456b654875c857e9f868d7619798767",
                AccessTokenLifeTime = -100
            });
            
            var tokenService = new TokenService(tokenRepository.Object, options);
            var expectedToken = await tokenService.CreateAccessTokenAsync(userEntity);

            // Act and Assert
            Assert.ThrowsAsync<SecurityTokenExpiredException>(() => tokenService.VerifyAccessTokenAsync(expectedToken.ToBase64String()));
        }

        [Test]
        public async Task VerifyRefreshTokenAsync_ExpiredToken_ShouldThrowSecurityTokenExpiredException()
        {
            // Arrange
            var userEntity = new UserEntity
            {
                Username = "test",
                Roles = new[] { Roles.Admin }
            };
            var tokenRepository = new Mock<ITokenRepository>();
            var options = Options.Create(new JwtConfiguration
            {
                Secret = "69102919d888ae3439bd7b1476c0e28d3456b654875c857e9f868d7619798767",
                RefreshTokenLifeTime = -100
            });
            
            var tokenService = new TokenService(tokenRepository.Object, options);
            var expectedToken = await tokenService.CreateRefreshTokenAsync(userEntity);

            // Act and Assert
            Assert.ThrowsAsync<SecurityTokenExpiredException>(() => tokenService.VerifyRefreshTokenAsync(expectedToken.ToBase64String()));
        }
        
        [Test]
        public async Task VerifyAccessTokenAsync_InvalidJti_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var userEntity = new UserEntity
            {
                Username = "test",
                Roles = new[] { Roles.Admin }
            };
            var tokenRepository = new Mock<ITokenRepository>();
            var options = Options.Create(new JwtConfiguration
            {
                Secret = "69102919d888ae3439bd7b1476c0e28d3456b654875c857e9f868d7619798767",
                AccessTokenLifeTime = 100
            });
            
            var tokenService = new TokenService(tokenRepository.Object, options);
            var expectedToken = await tokenService.CreateAccessTokenAsync(userEntity);

            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => tokenService.VerifyAccessTokenAsync(expectedToken.ToBase64String()));
        }
        
        [Test]
        public async Task VerifyRefreshTokenAsync_InvalidJti_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var userEntity = new UserEntity
            {
                Username = "test",
                Roles = new[] { Roles.Admin }
            };
            var tokenRepository = new Mock<ITokenRepository>();
            var options = Options.Create(new JwtConfiguration
            {
                Secret = "69102919d888ae3439bd7b1476c0e28d3456b654875c857e9f868d7619798767",
                RefreshTokenLifeTime = 100
            });
            
            var tokenService = new TokenService(tokenRepository.Object, options);
            var expectedToken = await tokenService.CreateRefreshTokenAsync(userEntity);

            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => tokenService.VerifyRefreshTokenAsync(expectedToken.ToBase64String()));
        }
        
        [Test]
        public async Task VerifyAccessTokenAsync_ValidToken_ShouldReturnClaimsPrincipal()
        {
            // Arrange
            var userEntity = new UserEntity
            {
                Username = "test",
                Roles = new[] { Roles.Admin }
            };
            var tokenRepository = new Mock<ITokenRepository>();
            var options = Options.Create(new JwtConfiguration
            {
                Secret = "69102919d888ae3439bd7b1476c0e28d3456b654875c857e9f868d7619798767",
                AccessTokenLifeTime = 100
            });
            
            var tokenService = new TokenService(tokenRepository.Object, options);
            var expectedToken = await tokenService.CreateAccessTokenAsync(userEntity);
            var expectedJti = expectedToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            tokenRepository.Setup(x => x.GetAccessTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(expectedJti));

            // Act
            var result = await tokenService.VerifyAccessTokenAsync(expectedToken.ToBase64String());
            
            // Act and Assert
            Assert.NotNull(result);
        }
        
        [Test]
        public async Task VerifyRefreshTokenAsync_ValidToken_ShouldReturnClaimsPrincipal()
        {
            // Arrange
            var userEntity = new UserEntity
            {
                Username = "test",
                Roles = new[] { Roles.Admin }
            };
            var tokenRepository = new Mock<ITokenRepository>();
            var options = Options.Create(new JwtConfiguration
            {
                Secret = "69102919d888ae3439bd7b1476c0e28d3456b654875c857e9f868d7619798767",
                RefreshTokenLifeTime = 100
            });
            
            var tokenService = new TokenService(tokenRepository.Object, options);
            var expectedToken = await tokenService.CreateRefreshTokenAsync(userEntity);
            var expectedJti = expectedToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            tokenRepository.Setup(x => x.GetRefreshTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(expectedJti));

            // Act
            var result = await tokenService.VerifyRefreshTokenAsync(expectedToken.ToBase64String());
            
            // Act and Assert
            Assert.NotNull(result);
        }

        [Test]
        public async Task InvalidateAccessTokenAsync_ValidDetails_ShouldCallDelete()
        {
            // Arrange
            var tokenRepository = new Mock<ITokenRepository>();
            var options = Options.Create(new JwtConfiguration());
            var token = "test";
            var tokenService = new TokenService(tokenRepository.Object, options);
            
            // Act
            await tokenService.InvalidateAccessTokenAsync(token);
            
            // Assert
            tokenRepository.Verify(x => x.DeleteAccessTokenAsync(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public async Task InvalidateRefreshTokenAsync_ValidDetails_ShouldCallDelete()
        {
            // Arrange
            var tokenRepository = new Mock<ITokenRepository>();
            var options = Options.Create(new JwtConfiguration());
            var token = "test";
            var tokenService = new TokenService(tokenRepository.Object, options);
            
            // Act
            await tokenService.InvalidRefreshTokenAsync(token);
            
            // Assert
            tokenRepository.Verify(x => x.DeleteRefreshTokenAsync(It.IsAny<string>()), Times.Once);
        }
    }
}