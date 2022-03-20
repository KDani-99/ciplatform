using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Configuration;
using CIPlatformManager.Entities;
using CIPlatformManager.Exceptions;
using CIPlatformManager.Services.Auth;
using CIPlatformManager.Services.Jobs;
using CIPlatformManager.Services.Workers;
using CIPlatformManager.SignalR.Hubs;
using CIPlatformManager.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CIPlatformManager.Tests.Services
{
    public class JobServiceTests
    {
        [Test]
        public void QueueJobAsync_NonExistentRun_ShouldThrowRunDoesNotExistException()
        {
            // Arrange
            var connectionId = "test-connection-id";
            var jobId = 0;
            var runId = 0;
            var jobDetails = new CachedJobDetails
            {
                RunId = runId,
                JobId = jobId
            };
            var busControl = new Mock<IBusControl>();
            var runRepository = new Mock<IRunRepository>();
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var hubContext = new Mock<IHubContext<WorkerHub, IWorkerClient>>();
            var workerConnectionService = new Mock<IWorkerConnectionService>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobService = new JobService(busControl.Object, runRepository.Object, tokenService.Object,
                hubContext.Object, workerConnectionService.Object, jsonSerializerOptions);

            // Act and Assert
            Assert.ThrowsAsync<RunDoesNotExistException>(() => jobService.QueueJobAsync(jobDetails, connectionId));
        }

        [Test]
        public void QueueJobAsync_NonExistentJob_ShouldThrowRunDoesNotExistException()
        {
            // Arrange
            var connectionId = "test-connection-id";
            var jobId = 0;
            var runId = 0;
            var jobDetails = new CachedJobDetails
            {
                RunId = runId,
                JobId = jobId
            };
            var busControl = new Mock<IBusControl>();
            var run = new RunEntity
            {
                Jobs = new List<JobEntity>()
            };
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var hubContext = new Mock<IHubContext<WorkerHub, IWorkerClient>>();
            var workerConnectionService = new Mock<IWorkerConnectionService>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobService = new JobService(busControl.Object, runRepository.Object, tokenService.Object,
                hubContext.Object, workerConnectionService.Object, jsonSerializerOptions);

            // Act and Assert
            Assert.ThrowsAsync<RunDoesNotExistException>(() => jobService.QueueJobAsync(jobDetails, connectionId));
        }

        [Test]
        public async Task QueueJobAsync_ValidDetails_ShouldQueueJobProperly()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionId = "test-connection-id";
            var jobId = 0;
            var runId = 0;
            var jobDetails = new CachedJobDetails
            {
                RunId = runId,
                JobId = jobId
            };
            var busControl = new Mock<IBusControl>();
            var job = new Mock<JobEntity>();
            job.Object.Id = jobId;
            job.Object.JsonContext = JsonSerializer.Serialize(new JobConfiguration());
            var run = new RunEntity
            {
                Id = runId,
                Jobs = new List<JobEntity>
                {
                    job.Object
                }
            };
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var tokenService = new TokenService(tokenServiceConfiguration);
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var tokenServiceMock = new Mock<ITokenService<JwtSecurityToken>>();
            tokenServiceMock.Setup(x => x.CreateJobTokenAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(tokenService.CreateJobTokenAsync(runId, jobId));
            var client = new Mock<IWorkerClient>();
            var clients = new Mock<IHubClients<IWorkerClient>>();
            clients.Setup(x => x.Client(It.IsAny<string>()))
                .Returns(client.Object);
            var hubContext = new Mock<IHubContext<WorkerHub, IWorkerClient>>();
            hubContext.SetupGet(x => x.Clients)
                .Returns(clients.Object);
            var workerConnectionDataEntity = new Mock<WorkerConnectionDataEntity>();
            var workerConnectionService = new Mock<IWorkerConnectionService>();
            workerConnectionService.Setup(x => x.GetWorkerConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(workerConnectionDataEntity.Object));
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobService = new JobService(busControl.Object, runRepository.Object, tokenServiceMock.Object,
                hubContext.Object, workerConnectionService.Object, jsonSerializerOptions);

            // Act
            await jobService.QueueJobAsync(jobDetails, connectionId);

            // Assert
            runRepository.Verify(x => x.GetAsync(It.IsAny<long>()), Times.Once);
            runRepository.Verify(x => x.UpdateAsync(It.IsAny<RunEntity>()), Times.Once);
            Assert.AreEqual(job.Object.State, States.Running);
            Assert.AreEqual(workerConnectionDataEntity.Object.WorkerState, WorkerState.Working);
        }

        [Test]
        public async Task QueueJobAsync_ValidDetails_ShouldSendJobRunningNotification()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionId = "test-connection-id";
            var jobId = 0;
            var runId = 0;
            var jobDetails = new CachedJobDetails
            {
                RunId = runId,
                JobId = jobId
            };
            var busControl = new Mock<IBusControl>();
            var job = new Mock<JobEntity>();
            job.Object.Id = jobId;
            job.Object.JsonContext = JsonSerializer.Serialize(new JobConfiguration());
            var run = new RunEntity
            {
                Id = runId,
                Jobs = new List<JobEntity>
                {
                    job.Object
                }
            };
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var tokenService = new TokenService(tokenServiceConfiguration);
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var tokenServiceMock = new Mock<ITokenService<JwtSecurityToken>>();
            tokenServiceMock.Setup(x => x.CreateJobTokenAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(tokenService.CreateJobTokenAsync(runId, jobId));
            var client = new Mock<IWorkerClient>();
            var clients = new Mock<IHubClients<IWorkerClient>>();
            clients.Setup(x => x.Client(It.IsAny<string>()))
                .Returns(client.Object);
            var hubContext = new Mock<IHubContext<WorkerHub, IWorkerClient>>();
            hubContext.SetupGet(x => x.Clients)
                .Returns(clients.Object);
            var workerConnectionDataEntity = new Mock<WorkerConnectionDataEntity>();
            var workerConnectionService = new Mock<IWorkerConnectionService>();
            workerConnectionService.Setup(x => x.GetWorkerConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(workerConnectionDataEntity.Object));
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobService = new JobService(busControl.Object, runRepository.Object, tokenServiceMock.Object,
                hubContext.Object, workerConnectionService.Object, jsonSerializerOptions);

            // Act
            await jobService.QueueJobAsync(jobDetails, connectionId);

            // Assert
            busControl.Verify(x => x.Publish(It.IsAny<ProcessedJobResultEvent>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task QueueJobAsync_ValidFirstJobDetails_ShouldSendRunRunningNotification()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionId = "test-connection-id";
            var jobId = 0;
            var runId = 0;
            var jobDetails = new CachedJobDetails
            {
                RunId = runId,
                JobId = jobId
            };
            var busControl = new Mock<IBusControl>();
            var job = new Mock<JobEntity>();
            job.Object.Id = jobId;
            job.Object.JsonContext = JsonSerializer.Serialize(new JobConfiguration());
            var run = new RunEntity
            {
                Id = runId,
                Jobs = new List<JobEntity>
                {
                    job.Object
                }
            };
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var tokenService = new TokenService(tokenServiceConfiguration);
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var tokenServiceMock = new Mock<ITokenService<JwtSecurityToken>>();
            tokenServiceMock.Setup(x => x.CreateJobTokenAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(tokenService.CreateJobTokenAsync(runId, jobId));
            var client = new Mock<IWorkerClient>();
            var clients = new Mock<IHubClients<IWorkerClient>>();
            clients.Setup(x => x.Client(It.IsAny<string>()))
                .Returns(client.Object);
            var hubContext = new Mock<IHubContext<WorkerHub, IWorkerClient>>();
            hubContext.SetupGet(x => x.Clients)
                .Returns(clients.Object);
            var workerConnectionDataEntity = new Mock<WorkerConnectionDataEntity>();
            var workerConnectionService = new Mock<IWorkerConnectionService>();
            workerConnectionService.Setup(x => x.GetWorkerConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(workerConnectionDataEntity.Object));
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobService = new JobService(busControl.Object, runRepository.Object, tokenServiceMock.Object,
                hubContext.Object, workerConnectionService.Object, jsonSerializerOptions);

            // Act
            await jobService.QueueJobAsync(jobDetails, connectionId);

            // Assert
            busControl.Verify(
                x => x.Publish(It.Is<ProcessedRunResultEvent>(p => p.State == States.Running),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task QueueJobAsync_ValidNonFirstJobDetails_ShouldNotSendRunRunningNotification()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionId = "test-connection-id";
            var jobId = 0;
            var jobId2 = 1;
            var runId = 0;
            var jobDetails = new CachedJobDetails
            {
                RunId = runId,
                JobId = jobId
            };
            var busControl = new Mock<IBusControl>();
            var job = new Mock<JobEntity>();
            job.Object.Id = jobId;
            job.Object.JsonContext = JsonSerializer.Serialize(new JobConfiguration());
            var job2 = new Mock<JobEntity>();
            job2.Object.Id = jobId2;
            job2.Object.JsonContext = JsonSerializer.Serialize(new JobConfiguration());
            var run = new RunEntity
            {
                Id = runId,
                Jobs = new List<JobEntity>
                {
                    job2.Object,
                    job.Object
                }
            };
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var tokenService = new TokenService(tokenServiceConfiguration);
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var tokenServiceMock = new Mock<ITokenService<JwtSecurityToken>>();
            tokenServiceMock.Setup(x => x.CreateJobTokenAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(tokenService.CreateJobTokenAsync(runId, jobId));
            var client = new Mock<IWorkerClient>();
            var clients = new Mock<IHubClients<IWorkerClient>>();
            clients.Setup(x => x.Client(It.IsAny<string>()))
                .Returns(client.Object);
            var hubContext = new Mock<IHubContext<WorkerHub, IWorkerClient>>();
            hubContext.SetupGet(x => x.Clients)
                .Returns(clients.Object);
            var workerConnectionDataEntity = new Mock<WorkerConnectionDataEntity>();
            var workerConnectionService = new Mock<IWorkerConnectionService>();
            workerConnectionService.Setup(x => x.GetWorkerConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(workerConnectionDataEntity.Object));
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jobService = new JobService(busControl.Object, runRepository.Object, tokenServiceMock.Object,
                hubContext.Object, workerConnectionService.Object, jsonSerializerOptions);
            
            // Act
            await jobService.QueueJobAsync(jobDetails, connectionId);

            // Assert
            busControl.Verify(
                x => x.Publish(It.Is<ProcessedRunResultEvent>(p => p.State == States.Running),
                    It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}