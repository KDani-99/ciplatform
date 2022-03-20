using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatformManager.Cache;
using CIPlatformManager.Repositories.Jobs;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace CIPlatformManager.Tests.Repositories
{
    public class JobQueueRepositoryTests
    {
        [Test]
        public async Task AddAsync_Front_ShouldCallListLeftPushAsync()
        {
            // Arrange
            var jobData = "test-job-data";
            var front = true;
            var jobContext = JobContext.Docker;

            var database = new Mock<IDatabase>();
            var redisJobQueueCache = new Mock<IRedisJobQueueCache>();
            redisJobQueueCache.SetupGet(x => x.Database)
                .Returns(database.Object);

            var jobQueueRepository = new JobQueueRepository(redisJobQueueCache.Object);

            // Act
            await jobQueueRepository.AddAsync(jobData, jobContext, front);

            // Assert
            database.Setup(x => x.ListLeftPushAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<When>(),
                It.IsAny<CommandFlags>()));
        }
        
        [Test]
        public async Task AddAsync_Back_ShouldCallListRightPushAsync()
        {
            // Arrange
            var jobData = "test-job-data";
            var front = false;
            var jobContext = JobContext.Docker;

            var database = new Mock<IDatabase>();
            var redisJobQueueCache = new Mock<IRedisJobQueueCache>();
            redisJobQueueCache.SetupGet(x => x.Database)
                .Returns(database.Object);

            var jobQueueRepository = new JobQueueRepository(redisJobQueueCache.Object);

            // Act
            await jobQueueRepository.AddAsync(jobData, jobContext, front);

            // Assert
            database.Setup(x => x.ListRightPushAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<When>(),
                It.IsAny<CommandFlags>()));
        }
    }
}