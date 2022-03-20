using System.Text.Json;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatformManager.Entities;
using CIPlatformManager.Repositories.Jobs;
using CIPlatformManager.Services.Jobs;
using Moq;
using NUnit.Framework;

namespace CIPlatformManager.Tests.Services
{
    public class JobQueueServiceTests
    {
        [Test]
        public async Task QueueAsync_DockerJobDetails_ShouldAddDockerJobDetailsToRepository()
        {
            // Arrange
            var jobQueueRepository = new Mock<IJobQueueRepository>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobQueueService = new JobQueueService(jobQueueRepository.Object, jsonSerializerOptions);
            var cachedJobDetails = new CachedJobDetails
            {
                JobId = 0,
                RunId = 0
            };
            var jobContext = JobContext.Docker;
            
            // Act
            await jobQueueService.QueueAsync(cachedJobDetails, jobContext);
            
            // Assert
            jobQueueRepository.Verify(x => x.AddAsync(It.IsAny<string>(), JobContext.Docker, false), Times.Once);
        }
        
        [Test]
        public async Task QueueAsync_LinuxJobDetails_ShouldAddLinuxJobDetailsToRepository()
        {
            // Arrange
            var jobQueueRepository = new Mock<IJobQueueRepository>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobQueueService = new JobQueueService(jobQueueRepository.Object, jsonSerializerOptions);
            var cachedJobDetails = new CachedJobDetails
            {
                JobId = 0,
                RunId = 0
            };
            var jobContext = JobContext.Linux;
            
            // Act
            await jobQueueService.QueueAsync(cachedJobDetails, jobContext);
            
            // Assert
            jobQueueRepository.Verify(x => x.AddAsync(It.IsAny<string>(), JobContext.Linux, false), Times.Once);
        }
        
        [Test]
        public async Task QueueAsync_WindowsJobDetails_ShouldAddWindowsJobDetailsToRepository()
        {
            // Arrange
            var jobQueueRepository = new Mock<IJobQueueRepository>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobQueueService = new JobQueueService(jobQueueRepository.Object, jsonSerializerOptions);
            var cachedJobDetails = new CachedJobDetails
            {
                JobId = 0,
                RunId = 0
            };
            var jobContext = JobContext.Windows;
            
            // Act
            await jobQueueService.QueueAsync(cachedJobDetails, jobContext);
            
            // Assert
            jobQueueRepository.Verify(x => x.AddAsync(It.IsAny<string>(), JobContext.Windows, false), Times.Once);
        }
        
        [Test]
        public async Task AddFrontAsync_DockerJobDetails_ShouldAddDockerJobDetailsToRepository()
        {
            // Arrange
            var jobQueueRepository = new Mock<IJobQueueRepository>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobQueueService = new JobQueueService(jobQueueRepository.Object, jsonSerializerOptions);
            var cachedJobDetails = new CachedJobDetails
            {
                JobId = 0,
                RunId = 0
            };
            var jobContext = JobContext.Docker;
            
            // Act
            await jobQueueService.AddFrontAsync(cachedJobDetails, jobContext);
            
            // Assert
            jobQueueRepository.Verify(x => x.AddAsync(It.IsAny<string>(), JobContext.Docker, true), Times.Once);
        }
        
        [Test]
        public async Task AddFrontAsync_LinuxJobDetails_ShouldAddLinuxJobDetailsToRepository()
        {
            // Arrange
            var jobQueueRepository = new Mock<IJobQueueRepository>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobQueueService = new JobQueueService(jobQueueRepository.Object, jsonSerializerOptions);
            var cachedJobDetails = new CachedJobDetails
            {
                JobId = 0,
                RunId = 0
            };
            var jobContext = JobContext.Linux;
            
            // Act
            await jobQueueService.AddFrontAsync(cachedJobDetails, jobContext);
            
            // Assert
            jobQueueRepository.Verify(x => x.AddAsync(It.IsAny<string>(), JobContext.Linux, true), Times.Once);
        }
        
        [Test]
        public async Task AddFrontAsync_WindowsJobDetails_ShouldAddWindowsJobDetailsToRepository()
        {
            // Arrange
            var jobQueueRepository = new Mock<IJobQueueRepository>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobQueueService = new JobQueueService(jobQueueRepository.Object, jsonSerializerOptions);
            var cachedJobDetails = new CachedJobDetails
            {
                JobId = 0,
                RunId = 0
            };
            var jobContext = JobContext.Linux;
            
            // Act
            await jobQueueService.AddFrontAsync(cachedJobDetails, jobContext);
            
            // Assert
            jobQueueRepository.Verify(x => x.AddAsync(It.IsAny<string>(), JobContext.Linux, true), Times.Once);
        }
        
        [Test]
        public async Task DequeueAsync_LinuxJobDetails_ShouldCallRemoveWithLinuxJobContext()
        {
            // Arrange
            var jobQueueRepository = new Mock<IJobQueueRepository>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobQueueService = new JobQueueService(jobQueueRepository.Object, jsonSerializerOptions);
            var jobContext = JobContext.Linux;
            
            // Act
            await jobQueueService.DequeueAsync(jobContext);
            
            // Assert
            jobQueueRepository.Verify(x => x.RemoveAsync(JobContext.Linux), Times.Once);
        }
        
        [Test]
        public async Task DequeueAsync_WindowsJobDetails_ShouldCallRemoveWithWindowsJobContext()
        {
            // Arrange
            var jobQueueRepository = new Mock<IJobQueueRepository>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobQueueService = new JobQueueService(jobQueueRepository.Object, jsonSerializerOptions);
            var jobContext = JobContext.Windows;
            
            // Act
            await jobQueueService.DequeueAsync(jobContext);
            
            // Assert
            jobQueueRepository.Verify(x => x.RemoveAsync(JobContext.Windows), Times.Once);
        }
        
        [Test]
        public async Task DequeueAsync_DockerJobDetails_ShouldCallRemoveWithDockerJobContext()
        {
            // Arrange
            var jobQueueRepository = new Mock<IJobQueueRepository>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobQueueService = new JobQueueService(jobQueueRepository.Object, jsonSerializerOptions);
            var jobContext = JobContext.Docker;
            
            // Act
            await jobQueueService.DequeueAsync(jobContext);
            
            // Assert
            jobQueueRepository.Verify(x => x.RemoveAsync(JobContext.Docker), Times.Once);
        }
    }
}