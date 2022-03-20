using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatformManager.Cache;
using CIPlatformManager.Repositories.Workers;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace CIPlatformManager.Tests.Repositories
{
    public class WorkerConnectionRepositoryTests
    {
        [Test]
        public async Task GetAsync_ValidDetails_ShouldReturnStringData()
        {
            // Arrange
            var connectionId = "connection";

            var database = new Mock<IDatabase>();
            var redisConnectionCache = new Mock<IRedisConnectionCache>();
            redisConnectionCache.SetupGet(x => x.Database)
                .Returns(database.Object);
                
            var workerConnectionRepository = new WorkerConnectionRepository(redisConnectionCache.Object);

            // Act
            await workerConnectionRepository.GetAsync(connectionId);

            // Assert
            database.Verify(x => x.HashGetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()));
        }
        
        [Test]
        public async Task AddAsync_ValidDetails_ShouldCallHashSetAsync()
        {
            // Arrange
            var connectionId = "connection";
            var workerData = "worker-data";

            var database = new Mock<IDatabase>();
            var redisConnectionCache = new Mock<IRedisConnectionCache>();
            redisConnectionCache.SetupGet(x => x.Database)
                .Returns(database.Object);
                
            var workerConnectionRepository = new WorkerConnectionRepository(redisConnectionCache.Object);

            // Act
            await workerConnectionRepository.AddAsync(connectionId, workerData);

            // Assert
            database.Verify(x => x.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<When>(),It.IsAny<CommandFlags>()));
        }
        
        [Test]
        public async Task AddToPoolAsync_ValidDetails_ShouldCallListRightPushAsync()
        {
            // Arrange
            var connectionId = "connection";
            var jobContext = JobContext.Docker;

            var database = new Mock<IDatabase>();
            var redisConnectionCache = new Mock<IRedisConnectionCache>();
            redisConnectionCache.SetupGet(x => x.Database)
                .Returns(database.Object);
                
            var workerConnectionRepository = new WorkerConnectionRepository(redisConnectionCache.Object);

            // Act
            await workerConnectionRepository.AddToPoolAsync(jobContext, connectionId);

            // Assert
            database.Verify(x => x.ListRightPushAsync(It.IsAny<RedisKey>(),  It.IsAny<RedisValue>(), It.IsAny<When>(),It.IsAny<CommandFlags>()));
        }
        
        [Test]
        public async Task RemoveAsync_ValidDetails_ShouldCallHashDeleteAsync()
        {
            // Arrange
            var connectionId = "connection";

            var database = new Mock<IDatabase>();
            var redisConnectionCache = new Mock<IRedisConnectionCache>();
            redisConnectionCache.SetupGet(x => x.Database)
                .Returns(database.Object);
                
            var workerConnectionRepository = new WorkerConnectionRepository(redisConnectionCache.Object);

            // Act
            await workerConnectionRepository.RemoveAsync(connectionId);

            // Assert
            database.Verify(x => x.HashDeleteAsync(It.IsAny<RedisKey>(),  It.IsAny<RedisValue>(),It.IsAny<CommandFlags>()));
        }
        
        [Test]
        public async Task RemoveFromPoolAsync_ValidDetails_ShouldCallListRemoveAsync()
        {
            // Arrange
            var connectionId = "connection";
            var jobContext = JobContext.Docker;

            var database = new Mock<IDatabase>();
            var redisConnectionCache = new Mock<IRedisConnectionCache>();
            redisConnectionCache.SetupGet(x => x.Database)
                .Returns(database.Object);
                
            var workerConnectionRepository = new WorkerConnectionRepository(redisConnectionCache.Object);

            // Act
            await workerConnectionRepository.RemoveFromPoolAsync(jobContext, connectionId);

            // Assert
            database.Verify(x => x.ListRemoveAsync(It.IsAny<RedisKey>(),  It.IsAny<RedisValue>(), It.IsAny<long>(), It.IsAny<CommandFlags>()));
        }
        
        [Test]
        public async Task RemoveFromPoolAsync_ValidDetails_ShouldCallListLeftPopAsync()
        {
            // Arrange
            var jobContext = JobContext.Docker;

            var database = new Mock<IDatabase>();
            var redisConnectionCache = new Mock<IRedisConnectionCache>();
            redisConnectionCache.SetupGet(x => x.Database)
                .Returns(database.Object);
                
            var workerConnectionRepository = new WorkerConnectionRepository(redisConnectionCache.Object);

            // Act
            await workerConnectionRepository.RemoveFromPoolAsync(jobContext);

            // Assert
            database.Verify(x => x.ListLeftPopAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()));
        }
        
        [Test]
        public async Task UpdateAsync_ValidDetails_ShouldCallHashSetAsync()
        {
            // Arrange
            var connectionId = "connection";
            var workerData = "worker-data";

            var database = new Mock<IDatabase>();
            var redisConnectionCache = new Mock<IRedisConnectionCache>();
            redisConnectionCache.SetupGet(x => x.Database)
                .Returns(database.Object);
                
            var workerConnectionRepository = new WorkerConnectionRepository(redisConnectionCache.Object);

            // Act
            await workerConnectionRepository.UpdateAsync(connectionId, workerData);

            // Assert
            database.Verify(x => x.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<When>(),It.IsAny<CommandFlags>()));
        }
        
        [Test]
        public async Task GetAllAsync_ValidDetails_ShouldCallListRangeAsync()
        {
            // Arrange
            var jobContext = JobContext.Docker;

            var database = new Mock<IDatabase>();
            var redisConnectionCache = new Mock<IRedisConnectionCache>();
            redisConnectionCache.SetupGet(x => x.Database)
                .Returns(database.Object);
                
            var workerConnectionRepository = new WorkerConnectionRepository(redisConnectionCache.Object);

            // Act
            await workerConnectionRepository.GetAllAsync(jobContext);

            // Assert
            database.Verify(x => x.ListRangeAsync(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<long>(),It.IsAny<CommandFlags>()));
        }
    }
}