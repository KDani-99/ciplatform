using System;
using System.Threading;
using System.Threading.Tasks;
using CIPlatformWebApi.Cache;
using CIPlatformWebApi.Repositories;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace CIPlatformWebAPI.Tests.Repositories
{
    public class TokenRepositoryTests
    {
        [Test]
        public async Task GetAccessTokenAsync_InvalidUsername_ShouldReturnNull()
        {
            // Arrange
            var username = "test";

            var database = new Mock<IDatabase>();
            var tokenCache = new Mock<ITokenCache>();
            tokenCache.SetupGet(x => x.Database)
                .Returns(database.Object);
            
            var tokenRepository = new TokenRepository(tokenCache.Object);
            
            // Act
            var result = await tokenRepository.GetAccessTokenAsync(username);
            
            // Assert
            Assert.IsNull(result);
            database.Verify(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Once);
        }
        
        [Test]
        public async Task GetAccessTokenAsync_ValidUsername_ShouldReturnValue()
        {
            // Arrange
            var username = "test";
            var token = "abc-123";

            var database = new Mock<IDatabase>();
            database.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .Returns(Task.FromResult(new RedisValue(token)));
            
            var tokenCache = new Mock<ITokenCache>();
            tokenCache.SetupGet(x => x.Database)
                .Returns(database.Object);
            
            var tokenRepository = new TokenRepository(tokenCache.Object);
            
            // Act
            var result = await tokenRepository.GetAccessTokenAsync(username);
            
            // Assert
            Assert.IsNotNull(result);
            database.Verify(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Once);
        }
        
        [Test]
        public async Task GetRefreshTokenAsync_InvalidUsername_ShouldReturnNull()
        {
            // Arrange
            var username = "test";

            var database = new Mock<IDatabase>();
            var tokenCache = new Mock<ITokenCache>();
            tokenCache.SetupGet(x => x.Database)
                .Returns(database.Object);
            
            var tokenRepository = new TokenRepository(tokenCache.Object);
            
            // Act
            var result = await tokenRepository.GetRefreshTokenAsync(username);
            
            // Assert
            Assert.IsNull(result);
            database.Verify(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Once);
        }
        
        [Test]
        public async Task GetRefreshTokenAsync_ValidUsername_ShouldReturnValue()
        {
            // Arrange
            var username = "test";
            var token = "abc-123";

            var database = new Mock<IDatabase>();
            database.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .Returns(Task.FromResult(new RedisValue(token)));
            
            var tokenCache = new Mock<ITokenCache>();
            tokenCache.SetupGet(x => x.Database)
                .Returns(database.Object);
            
            var tokenRepository = new TokenRepository(tokenCache.Object);
            
            // Act
            var result = await tokenRepository.GetRefreshTokenAsync(username);
            
            // Assert
            Assert.IsNotNull(result);
            database.Verify(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Once);
        }
        
        [Test]
        public async Task AddAccessTokenAsync_ValidDetails_ShouldSucceed()
        {
            // Arrange
            var username = "test";
            var token = "abc-123";
            var timeSpan = TimeSpan.Zero;

            var database = new Mock<IDatabase>();
            database.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), It.IsAny<When>(),It.IsAny<CommandFlags>()))
                .Returns(Task.FromResult(true));
            
            var tokenCache = new Mock<ITokenCache>();
            tokenCache.SetupGet(x => x.Database)
                .Returns(database.Object);
            
            var tokenRepository = new TokenRepository(tokenCache.Object);
            
            // Act
            var result = await tokenRepository.AddAccessTokenAsync(username, token, timeSpan);
            
            // Assert
            Assert.True(result);
            database.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), It.IsAny<When>(),It.IsAny<CommandFlags>()), Times.Once);
        }
        
        [Test]
        public async Task AddRefreshTokenAsync_ValidDetails_ShouldSucceed()
        {
            // Arrange
            var username = "test";
            var token = "abc-123";
            var timeSpan = TimeSpan.Zero;

            var database = new Mock<IDatabase>();
            database.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), It.IsAny<When>(),It.IsAny<CommandFlags>()))
                .Returns(Task.FromResult(true));
            
            var tokenCache = new Mock<ITokenCache>();
            tokenCache.SetupGet(x => x.Database)
                .Returns(database.Object);
            
            var tokenRepository = new TokenRepository(tokenCache.Object);
            
            // Act
            var result = await tokenRepository.AddRefreshTokenAsync(username, token, timeSpan);
            
            // Assert
            Assert.True(result);
            database.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), It.IsAny<When>(),It.IsAny<CommandFlags>()), Times.Once);
        }
        
        [Test]
        public async Task DeleteAccessTokenAsync_ValidDetails_ShouldSucceed()
        {
            // Arrange
            var username = "test";

            var database = new Mock<IDatabase>();
            database.Setup(x => x.KeyDeleteAsync(It.IsAny<RedisKey>() ,It.IsAny<CommandFlags>()))
                .Returns(Task.FromResult(true));
            
            var tokenCache = new Mock<ITokenCache>();
            tokenCache.SetupGet(x => x.Database)
                .Returns(database.Object);
            
            var tokenRepository = new TokenRepository(tokenCache.Object);
            
            // Act
            var result = await tokenRepository.DeleteAccessTokenAsync(username);
            
            // Assert
            Assert.True(result);
            database.Verify(x => x.KeyDeleteAsync(It.IsAny<RedisKey>() ,It.IsAny<CommandFlags>()), Times.Once);
        }
        
        [Test]
        public async Task DeleteRefreshTokenAsync_ValidDetails_ShouldSucceed()
        {
            // Arrange
            var username = "test";

            var database = new Mock<IDatabase>();
            database.Setup(x => x.KeyDeleteAsync(It.IsAny<RedisKey>() ,It.IsAny<CommandFlags>()))
                .Returns(Task.FromResult(true));
            
            var tokenCache = new Mock<ITokenCache>();
            tokenCache.SetupGet(x => x.Database)
                .Returns(database.Object);
            
            var tokenRepository = new TokenRepository(tokenCache.Object);
            
            // Act
            var result = await tokenRepository.DeleteRefreshTokenAsync(username);
            
            // Assert
            Assert.True(result);
            database.Verify(x => x.KeyDeleteAsync(It.IsAny<RedisKey>() ,It.IsAny<CommandFlags>()), Times.Once);
        }
    }
}