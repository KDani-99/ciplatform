using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Events;
using CIPlatform.Data.Extensions;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Configuration;
using CIPlatformManager.Services.Auth;
using CIPlatformManager.Services.Steps;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CIPlatformManager.Tests.Services
{
    public class StepServiceTests
    {
        [Test]
        public async Task ProcessStepResultAsync_RunningStepResult_ShouldSendStepNotificationWithRunningState()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionId = "0239544";
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var stepIndex = 0;
            var step = new StepEntity
            {
                Id = stepId,
                Index = stepIndex
            };
            var job = new JobEntity
            {
                Id = jobId,
                Steps = new List<StepEntity> {step}
            };
            var project = new ProjectEntity();
            var run = new RunEntity
            {
                Id = runId,
                Jobs = new List<JobEntity>
                {
                    job
                },
                Project = project
            };
            var tokenService = new TokenService(tokenServiceConfiguration);
            var token = await tokenService.CreateJobTokenAsync(runId, jobId);
            var claims = await tokenService.VerifyJobTokenAsync(token.ToBase64String());
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var busControl = new Mock<IBusControl>(); ;
            var tokenServiceMock = new Mock<ITokenService<JwtSecurityToken>>();
            tokenServiceMock.Setup(x => x.VerifyJobTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(claims));
            var stepService = new StepService(busControl.Object, runRepository.Object, tokenServiceMock.Object);
            var stepResultEvent = new StepResultEvent
            {
                Token = token.ToBase64String(),
                StepIndex = stepIndex,
                State = States.Running
            };
            // Act
            await stepService.ProcessStepResultAsync(stepResultEvent, connectionId);

            // Assert
            busControl.Verify(x => x.Publish(It.Is<ProcessedStepResultEvent>(x => x.State == States.Running), It.IsAny<CancellationToken>()));
            runRepository.Verify(x => x.UpdateAsync(It.IsAny<RunEntity>()), Times.Once);
        }
        
         [Test]
        public async Task ProcessStepResultAsync_SuccessfulStepResult_ShouldIncreaseNumberOfCompletedStepsByOne()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionId = "0239544";
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var stepIndex = 0;
            var step = new StepEntity
            {
                Id = stepId,
                Index = stepIndex
            };
            var job = new JobEntity
            {
                Id = jobId,
                Steps = new List<StepEntity> {step}
            };
            var project = new ProjectEntity();
            var run = new RunEntity
            {
                Id = runId,
                Jobs = new List<JobEntity>
                {
                    job
                },
                Project = project,
                NumberOfSteps = 1,
                NumberOfCompletedSteps = 0
            };
            var tokenService = new TokenService(tokenServiceConfiguration);
            var token = await tokenService.CreateJobTokenAsync(runId, jobId);
            var claims = await tokenService.VerifyJobTokenAsync(token.ToBase64String());
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var busControl = new Mock<IBusControl>(); ;
            var tokenServiceMock = new Mock<ITokenService<JwtSecurityToken>>();
            tokenServiceMock.Setup(x => x.VerifyJobTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(claims));
            var stepService = new StepService(busControl.Object, runRepository.Object, tokenServiceMock.Object);
            var stepResultEvent = new StepResultEvent
            {
                Token = token.ToBase64String(),
                StepIndex = stepIndex,
                State = States.Successful
            };
            var expectedCompletedSteps = 1;
            // Act
            await stepService.ProcessStepResultAsync(stepResultEvent, connectionId);

            // Assert
            runRepository.Verify(x => x.UpdateAsync(It.IsAny<RunEntity>()), Times.Once);
            Assert.AreEqual(expectedCompletedSteps, run.NumberOfCompletedSteps);
        }
        
        [Test]
        public async Task ProcessStepResultAsync_RunningStepResult_ShouldSendJobAndStepNotificationWithRunningState()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionId = "0239544";
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var stepIndex = 0;
            var step = new StepEntity
            {
                Id = stepId,
                Index = stepIndex,
            };
            var job = new JobEntity
            {
                Id = jobId,
                Steps = new List<StepEntity> {step},
                State = States.NotRun
            };
            var project = new ProjectEntity();
            var run = new RunEntity
            {
                Id = runId,
                Jobs = new List<JobEntity>
                {
                    job
                },
                Project = project,
                State = States.NotRun
            };
            var tokenService = new TokenService(tokenServiceConfiguration);
            var token = await tokenService.CreateJobTokenAsync(runId, jobId);
            var claims = await tokenService.VerifyJobTokenAsync(token.ToBase64String());
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var busControl = new Mock<IBusControl>(); ;
            var tokenServiceMock = new Mock<ITokenService<JwtSecurityToken>>();
            tokenServiceMock.Setup(x => x.VerifyJobTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(claims));
            var stepService = new StepService(busControl.Object, runRepository.Object, tokenServiceMock.Object);
            var stepResultEvent = new StepResultEvent
            {
                Token = token.ToBase64String(),
                StepIndex = stepIndex,
                State = States.Running
            };
            // Act
            await stepService.ProcessStepResultAsync(stepResultEvent, connectionId);

            // Assert
            busControl.Verify(x => x.Publish(It.Is<ProcessedStepResultEvent>(s => s.State == States.Running), It.IsAny<CancellationToken>()));
            busControl.Verify(x => x.Publish(It.Is<ProcessedJobResultEvent>(j => j.State == States.Running), It.IsAny<CancellationToken>()));
            runRepository.Verify(x => x.UpdateAsync(It.IsAny<RunEntity>()), Times.Once);
        }
        
        [Test]
        public async Task ProcessStepResultAsync_RunningStepResult_ShouldSendRunNotificationWithRunningState()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionId = "0239544";
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var stepIndex = 0;
            var step = new StepEntity
            {
                Id = stepId,
                Index = stepIndex,
            };
            var job = new JobEntity
            {
                Id = jobId,
                Steps = new List<StepEntity> {step},
                State = States.NotRun
            };
            var project = new ProjectEntity();
            var run = new RunEntity
            {
                Id = runId,
                Jobs = new List<JobEntity>
                {
                    job
                },
                Project = project,
                State = States.Queued
            };
            var tokenService = new TokenService(tokenServiceConfiguration);
            var token = await tokenService.CreateJobTokenAsync(runId, jobId);
            var claims = await tokenService.VerifyJobTokenAsync(token.ToBase64String());
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var busControl = new Mock<IBusControl>(); ;
            var tokenServiceMock = new Mock<ITokenService<JwtSecurityToken>>();
            tokenServiceMock.Setup(x => x.VerifyJobTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(claims));
            var stepService = new StepService(busControl.Object, runRepository.Object, tokenServiceMock.Object);
            var stepResultEvent = new StepResultEvent
            {
                Token = token.ToBase64String(),
                StepIndex = stepIndex,
                State = States.Running
            };
            // Act
            await stepService.ProcessStepResultAsync(stepResultEvent, connectionId);

            // Assert
            busControl.Verify(x => x.Publish(It.Is<ProcessedRunResultEvent>(r => r.State == States.Running), It.IsAny<CancellationToken>()));
            runRepository.Verify(x => x.UpdateAsync(It.IsAny<RunEntity>()), Times.Once);
        }
        
         [Test]
        public async Task ProcessStepResultAsync_FailedStepResult_ShouldSendRunAndJobAndStepNotificationWithFailedState()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionId = "0239544";
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var stepIndex = 0;
            var step = new StepEntity
            {
                Id = stepId,
                Index = stepIndex
            };
            var job = new JobEntity
            {
                Id = jobId,
                Steps = new List<StepEntity> {step}
            };
            var project = new ProjectEntity();
            var run = new RunEntity
            {
                Id = runId,
                Jobs = new List<JobEntity>
                {
                    job
                },
                Project = project
            };
            var tokenService = new TokenService(tokenServiceConfiguration);
            var token = await tokenService.CreateJobTokenAsync(runId, jobId);
            var claims = await tokenService.VerifyJobTokenAsync(token.ToBase64String());
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var busControl = new Mock<IBusControl>(); ;
            var tokenServiceMock = new Mock<ITokenService<JwtSecurityToken>>();
            tokenServiceMock.Setup(x => x.VerifyJobTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(claims));
            var stepService = new StepService(busControl.Object, runRepository.Object, tokenServiceMock.Object);
            var stepResultEvent = new StepResultEvent
            {
                Token = token.ToBase64String(),
                StepIndex = stepIndex,
                State = States.Failed
            };
            // Act
            await stepService.ProcessStepResultAsync(stepResultEvent, connectionId);

            // Assert
            busControl.Verify(x => x.Publish(It.Is<ProcessedStepResultEvent>(s => s.State == States.Failed), It.IsAny<CancellationToken>()));
            busControl.Verify(x => x.Publish(It.Is<ProcessedRunResultEvent>(r => r.State == States.Failed), It.IsAny<CancellationToken>()));
            busControl.Verify(x => x.Publish(It.Is<ProcessedJobResultEvent>(j => j.State == States.Failed), It.IsAny<CancellationToken>()));
            runRepository.Verify(x => x.UpdateAsync(It.IsAny<RunEntity>()), Times.Once);
        }
        
        [Test]
        public async Task ProcessStepResultAsync_SuccessfulStepResult_ShouldSendRunAndJobAndStepNotificationWithSuccessfulState()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionId = "0239544";
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var stepIndex = 0;
            var step = new StepEntity
            {
                Id = stepId,
                Index = stepIndex
            };
            var job = new JobEntity
            {
                Id = jobId,
                Steps = new List<StepEntity> {step},
                State = States.Running
            };
            var project = new ProjectEntity();
            var run = new RunEntity
            {
                Id = runId,
                Jobs = new List<JobEntity>
                {
                    job
                },
                NumberOfSteps = 1,
                NumberOfCompletedSteps = 0,
                Project = project,
                State = States.Running
            };
            var tokenService = new TokenService(tokenServiceConfiguration);
            var token = await tokenService.CreateJobTokenAsync(runId, jobId);
            var claims = await tokenService.VerifyJobTokenAsync(token.ToBase64String());
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            var busControl = new Mock<IBusControl>(); ;
            var tokenServiceMock = new Mock<ITokenService<JwtSecurityToken>>();
            tokenServiceMock.Setup(x => x.VerifyJobTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(claims));
            var stepService = new StepService(busControl.Object, runRepository.Object, tokenServiceMock.Object);
            var stepResultEvent = new StepResultEvent
            {
                Token = token.ToBase64String(),
                StepIndex = stepIndex,
                State = States.Successful
            };
            // Act
            await stepService.ProcessStepResultAsync(stepResultEvent, connectionId);

            // Assert
            busControl.Verify(x => x.Publish(It.Is<ProcessedStepResultEvent>(s => s.State == States.Successful), It.IsAny<CancellationToken>()));
            busControl.Verify(x => x.Publish(It.Is<ProcessedRunResultEvent>(r => r.State == States.Successful), It.IsAny<CancellationToken>()));
            busControl.Verify(x => x.Publish(It.Is<ProcessedJobResultEvent>(j => j.State == States.Successful), It.IsAny<CancellationToken>()));
            runRepository.Verify(x => x.UpdateAsync(It.IsAny<RunEntity>()), Times.Once);
        }
    }
}