using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Exceptions;
using CIPlatformWorker.Services;
using CIPlatformWorker.Tests.Mocks;
using CIPlatformWorker.WebSocket;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CIPlatformWorker.Tests
{
    public class DockerJobHandlerServiceTests
    {
        [Test]
        public async Task PrepareEnvironmentAsync_CreateDockerContainer_ShouldThrowStepFailedException()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var jobConfiguration = new JobConfiguration
            {
                Image = "test:image",
                Environment = new List<string>(),
                Steps = new List<StepConfiguration>()
            };
            var cancellationToken = CancellationToken.None;
            var imagesCreateParameters = new Mock<ImagesCreateParameters>();
            var createContainerResponse = new CreateContainerResponse
            {
                ID = "test-docker-container-id"
            };
            var agentConfiguration = fixture.Create<IOptions<WorkerConfiguration>>();
            ;
            var workerClient = new Mock<IWorkerClient>();
            var dockerClient = new Mock<IDockerClient>();

            var hubConnection = new Mock<MockHubConnection>();

            workerClient
                .SetupGet(x => x.HubConnection)
                .Returns(hubConnection.Object);
            hubConnection
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            dockerClient
                .Setup(x => x.Images.CreateImageAsync(imagesCreateParameters.Object, jobConfiguration.AuthConfig,
                                                      new Progress<JSONMessage>(), cancellationToken))
                .Returns(Task.FromResult(createContainerResponse));
            dockerClient
                .Setup(x => x.Containers.CreateContainerAsync(It.IsAny<CreateContainerParameters>(),
                                                              It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(createContainerResponse));
            dockerClient
                .Setup(x => x.Containers.StartContainerAsync(It.IsAny<string>(), It.IsAny<ContainerStartParameters>(),
                                                             It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));
            await using var dockerJobHandlerService = new DockerJobHandlerService(
                jobConfiguration, agentConfiguration, dockerClient.Object, cancellationToken);

            // Act and Assert
            Assert.ThrowsAsync<StepFailedException>(
                async () => await dockerJobHandlerService.PrepareEnvironmentAsync());
        }

        [Test]
        public async Task PrepareEnvironmentAsync_CreateDockerContainer_ShouldCreateAndStartContainer()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var jobConfiguration = new JobConfiguration
            {
                Image = "test:image",
                Environment = new List<string>(),
                Steps = new List<StepConfiguration>()
            };
            var jobDetails = new Mock<JobDetails>();
            var cancellationToken = CancellationToken.None;
            var imagesCreateParameters = new Mock<ImagesCreateParameters>();
            var createContainerResponse = new CreateContainerResponse
            {
                ID = "test-docker-container-id"
            };
            var agentConfiguration = fixture.Create<IOptions<WorkerConfiguration>>();
            ;
            var workerClient = new Mock<IWorkerClient>();
            var dockerClient = new Mock<IDockerClient>();

            var hubConnection = new Mock<MockHubConnection>();

            workerClient
                .SetupGet(x => x.HubConnection)
                .Returns(hubConnection.Object);
            hubConnection
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            dockerClient
                .Setup(x => x.Images.CreateImageAsync(imagesCreateParameters.Object, jobConfiguration.AuthConfig,
                                                      new Progress<JSONMessage>(), cancellationToken))
                .Returns(Task.FromResult(createContainerResponse));
            dockerClient
                .Setup(x => x.Containers.CreateContainerAsync(It.IsAny<CreateContainerParameters>(),
                                                              It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(createContainerResponse));
            dockerClient
                .Setup(x => x.Containers.StartContainerAsync(It.IsAny<string>(), It.IsAny<ContainerStartParameters>(),
                                                             It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            await using var dockerJobHandlerService = new DockerJobHandlerService(
                jobDetails.Object, jobConfiguration, agentConfiguration, dockerClient.Object, cancellationToken);

            // Act
            await dockerJobHandlerService.PrepareEnvironmentAsync();

            // Assert
            dockerClient
                .Verify(
                    x => x.Images.CreateImageAsync(It.IsAny<ImagesCreateParameters>(), It.IsAny<AuthConfig>(),
                                                   It.IsAny<Progress<JSONMessage>>(), It.IsAny<CancellationToken>()),
                    Times.Exactly(1));
            dockerClient
                .Verify(
                    x => x.Containers.CreateContainerAsync(It.IsAny<CreateContainerParameters>(),
                                                           It.IsAny<CancellationToken>()), Times.Exactly(1));
            dockerClient
                .Verify(
                    x => x.Containers.StartContainerAsync(It.IsAny<string>(), It.IsAny<ContainerStartParameters>(),
                                                          It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Test]
        public async Task ExecuteStepAsync_CheckEnvironment_ShouldThrowEnvironmentNotPreparedException()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var jobConfiguration = new JobConfiguration
            {
                Image = "test:image",
                Environment = new List<string>(),
                Steps = new List<StepConfiguration>()
            };
            var jobDetails = new Mock<JobDetails>();
            var cancellationToken = CancellationToken.None;
            var agentConfiguration = fixture.Create<IOptions<WorkerConfiguration>>();
            ;
            var dockerClient = new Mock<IDockerClient>();
            var channelWriter = new Mock<ChannelWriter<string>>();
            var stepConfiguration = new Mock<StepConfiguration>();
            var stepIndex = 0;
            await using var dockerJobHandlerService = new DockerJobHandlerService(
                jobDetails.Object, jobConfiguration, agentConfiguration, dockerClient.Object, cancellationToken);

            // Act and Assert
            Assert.ThrowsAsync<EnvironmentNotPreparedException>(async () =>
                                                                    await dockerJobHandlerService.ExecuteStepAsync(
                                                                        channelWriter.Object, stepConfiguration.Object,
                                                                        stepIndex));
        }

        [Test]
        public async Task PrepareEnvironmentAsync_CreateDockerContainer_ShouldExecAndAttachToContainer()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var jobConfiguration = new JobConfiguration
            {
                Image = "test:image",
                Environment = new List<string>(),
                Steps = new List<StepConfiguration>()
            };
            var jobDetails = new Mock<JobDetails>();
            var cancellationToken = CancellationToken.None;
            var imagesCreateParameters = new Mock<ImagesCreateParameters>();
            var createContainerResponse = new CreateContainerResponse
            {
                ID = "test-docker-container-id"
            };
            var execContainerResponse = new ContainerExecCreateResponse
            {
                ID = "test-docker-exec-id"
            };
            var inspectContainerResponse = new ContainerExecInspectResponse
            {
                ExitCode = 0
            };
            var agentConfiguration = fixture.Create<IOptions<WorkerConfiguration>>();
            ;
            var workerClient = new Mock<IWorkerClient>();
            var dockerClient = new Mock<IDockerClient>();
            var channelWriter = new Mock<ChannelWriter<string>>();
            var stepConfiguration = new StepConfiguration
            {
                Cmd = "cmd with args"
            };
            var stepIndex = 0;
            var hubConnection = new Mock<MockHubConnection>();
            var stream = new Mock<Stream>();
            var multiplexedStream = new MultiplexedStream(stream.Object, true);

            workerClient
                .SetupGet(x => x.HubConnection)
                .Returns(hubConnection.Object);
            hubConnection
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            dockerClient
                .Setup(x => x.Images.CreateImageAsync(imagesCreateParameters.Object, jobConfiguration.AuthConfig,
                                                      new Progress<JSONMessage>(), cancellationToken))
                .Returns(Task.FromResult(createContainerResponse));
            dockerClient
                .Setup(x => x.Containers.CreateContainerAsync(It.IsAny<CreateContainerParameters>(),
                                                              It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(createContainerResponse));
            dockerClient
                .Setup(x => x.Containers.StartContainerAsync(It.IsAny<string>(), It.IsAny<ContainerStartParameters>(),
                                                             It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            dockerClient
                .Setup(x => x.Exec.ExecCreateContainerAsync(It.IsAny<string>(),
                                                            It.IsAny<ContainerExecCreateParameters>(),
                                                            It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(execContainerResponse));
            dockerClient
                .Setup(x => x.Exec.StartAndAttachContainerExecAsync(It.IsAny<string>(), It.IsAny<bool>(),
                                                                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(multiplexedStream));
            dockerClient
                .Setup(x => x.Exec.InspectContainerExecAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(inspectContainerResponse));
            await using var dockerJobHandlerService = new DockerJobHandlerService(
                jobDetails.Object, jobConfiguration, agentConfiguration, dockerClient.Object, cancellationToken);

            // Act
            await dockerJobHandlerService.PrepareEnvironmentAsync();

            await dockerJobHandlerService.ExecuteStepAsync(channelWriter.Object, stepConfiguration, stepIndex);

            // Assert
            dockerClient
                .Verify(
                    x => x.Exec.ExecCreateContainerAsync(It.IsAny<string>(), It.IsAny<ContainerExecCreateParameters>(),
                                                         It.IsAny<CancellationToken>()), Times.Exactly(1));
            dockerClient
                .Verify(
                    x => x.Exec.StartAndAttachContainerExecAsync(It.IsAny<string>(), It.IsAny<bool>(),
                                                                 It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Test]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(45564)]
        public async Task PrepareEnvironmentAsync_CreateDockerContainer_NonZeroExitCodShouldThrowStepFailedException(
            int exitCode)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var jobConfiguration = new JobConfiguration
            {
                Image = "test:image",
                Environment = new List<string>(),
                Steps = new List<StepConfiguration>()
            };
            var jobDetails = new Mock<JobDetails>();
            var cancellationToken = CancellationToken.None;
            var imagesCreateParameters = new Mock<ImagesCreateParameters>();
            var createContainerResponse = new CreateContainerResponse
            {
                ID = "test-docker-container-id"
            };
            var execContainerResponse = new ContainerExecCreateResponse
            {
                ID = "test-docker-exec-id"
            };
            var inspectContainerResponse = new ContainerExecInspectResponse
            {
                ExitCode = exitCode
            };
            var agentConfiguration = fixture.Create<IOptions<WorkerConfiguration>>();
            ;
            var workerClient = new Mock<IWorkerClient>();
            var dockerClient = new Mock<IDockerClient>();
            var channelWriter = new Mock<ChannelWriter<string>>();
            var stepConfiguration = new StepConfiguration
            {
                Cmd = "cmd with args"
            };
            var stepIndex = 0;
            var hubConnection = new Mock<MockHubConnection>();
            var stream = new Mock<Stream>();
            var multiplexedStream = new MultiplexedStream(stream.Object, true);

            workerClient
                .SetupGet(x => x.HubConnection)
                .Returns(hubConnection.Object);
            hubConnection
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            dockerClient
                .Setup(x => x.Images.CreateImageAsync(imagesCreateParameters.Object, jobConfiguration.AuthConfig,
                                                      new Progress<JSONMessage>(), cancellationToken))
                .Returns(Task.FromResult(createContainerResponse));
            dockerClient
                .Setup(x => x.Containers.CreateContainerAsync(It.IsAny<CreateContainerParameters>(),
                                                              It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(createContainerResponse));
            dockerClient
                .Setup(x => x.Containers.StartContainerAsync(It.IsAny<string>(), It.IsAny<ContainerStartParameters>(),
                                                             It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            dockerClient
                .Setup(x => x.Exec.ExecCreateContainerAsync(It.IsAny<string>(),
                                                            It.IsAny<ContainerExecCreateParameters>(),
                                                            It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(execContainerResponse));
            dockerClient
                .Setup(x => x.Exec.StartAndAttachContainerExecAsync(It.IsAny<string>(), It.IsAny<bool>(),
                                                                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(multiplexedStream));
            dockerClient
                .Setup(x => x.Exec.InspectContainerExecAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(inspectContainerResponse));
            await using var dockerJobHandlerService = new DockerJobHandlerService(
                jobDetails.Object, jobConfiguration, agentConfiguration, dockerClient.Object, cancellationToken);
            await dockerJobHandlerService.PrepareEnvironmentAsync();

            // Act and Assert
            Assert.ThrowsAsync<StepFailedException>(
                async () => await dockerJobHandlerService.ExecuteStepAsync(
                    channelWriter.Object, stepConfiguration, stepIndex));
        }
    }
}