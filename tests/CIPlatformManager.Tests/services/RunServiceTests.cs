using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CIPlatform.Data.Commands;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Entities;
using CIPlatformManager.Services.Jobs;
using CIPlatformManager.Services.Runs;
using MassTransit;
using Moq;
using NUnit.Framework;

namespace CIPlatformManager.Tests.Services
{
    public class RunServiceTests
    {
        [Test]
        public async Task QueueAsync_ValidCommand_ShouldQueueAllJobs()
        {
            // Arrange
            var runId = 0;
            var jobId = 0;
            var job2Id = 1;
            var projectId = 0;
            var queueRunCommand = new QueueRunCommand
            {
                RunId = runId
            };
            var job = new JobEntity
            {
                Id = jobId
            };
            var job2 = new JobEntity
            {
                Id = job2Id
            };
            var project = new ProjectEntity
            {
                Id = projectId
            };
            var run = new RunEntity
            {
                Jobs = new List<JobEntity>
                {
                    job,
                    job2
                },
                Project = project
            };
            
            var busControl = new Mock<IBusControl>();
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var jobQueueService = new Mock<IJobQueueService>();
            var runService = new RunService(busControl.Object, runRepository.Object, jobQueueService.Object);
            
            // Act
            await runService.QueueAsync(queueRunCommand);

            // Assert
            jobQueueService.Verify(x => x.QueueAsync(It.IsAny<CachedJobDetails>(), It.IsAny<JobContext>()),Times.Exactly(2));
        }
        
        [Test]
        public async Task QueueAsync_ValidCommand_ShouldSendRunQueuedNotification()
        {
            // Arrange
            var runId = 0;
            var jobId = 0;
            var job2Id = 1;
            var projectId = 0;
            var queueRunCommand = new QueueRunCommand
            {
                RunId = runId
            };
            var job = new JobEntity
            {
                Id = jobId
            };
            var job2 = new JobEntity
            {
                Id = job2Id
            };
            var project = new ProjectEntity
            {
                Id = projectId
            };
            var run = new RunEntity
            {
                Jobs = new List<JobEntity>
                {
                    job,
                    job2
                },
                Project = project
            };
            
            var busControl = new Mock<IBusControl>();
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var jobQueueService = new Mock<IJobQueueService>();
            var runService = new RunService(busControl.Object, runRepository.Object, jobQueueService.Object);
            
            // Act
            await runService.QueueAsync(queueRunCommand);

            // Assert
            busControl.Verify(x => x.Publish(It.IsAny<ProcessedRunResultEvent>(), It.IsAny<CancellationToken>()),Times.Once);
        }
    }
}