using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Services;
using CodeManagerAgent.Tests.Mocks;
using CodeManagerAgent.WebSocket;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Moq;
namespace CodeManagerAgent.Tests
{
    public class DockerJobHandlerServiceTests
    {
        [Test]
        public async Task StartAsync_CreateDockerContainer_ShouldCreateContainer()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var jobConfiguration = new JobConfiguration
            {
                Image = "test:image",
                Environment = new List<string>()
            };
            var jobDetails = new Mock<JobDetails>();
            var cancellationToken = CancellationToken.None;
            var imagesCreateParameters = new Mock<ImagesCreateParameters>();
            var createContainerResponse = new CreateContainerResponse
            {
                ID = "test-docker-container-id"
            };
            var agentConfiguration = fixture.Create<IOptions<AgentConfiguration>>();
            var dockerJobHandlerServiceLogger = new Mock<ILogger<DockerJobHandlerService>>();
            var workerClient = new Mock<IWorkerClient>();
            var dockerClient = new Mock<IDockerClient>();

            var hubConnection = new Mock<MockHubConnection>();

            workerClient
                .SetupGet(x => x.HubConnection)
                .Returns(hubConnection.Object); 
            hubConnection.Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            dockerClient
                .Setup(x => x.Images.CreateImageAsync(imagesCreateParameters.Object, jobConfiguration.AuthConfig, new Progress<JSONMessage>(), cancellationToken))
                .Returns(Task.FromResult(createContainerResponse));
           dockerClient
                .Setup(x => x.Containers.CreateContainerAsync(It.IsAny<CreateContainerParameters>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(createContainerResponse));
           await using var dockerJobHandlerService = new DockerJobHandlerService(jobDetails.Object, jobConfiguration,workerClient.Object,agentConfiguration, dockerClient.Object, dockerJobHandlerServiceLogger.Object, cancellationToken);
            // Act
            await dockerJobHandlerService.StartAsync();
            
            // Assert
            dockerClient
                .Verify((x) => x.Images.CreateImageAsync(It.IsAny<ImagesCreateParameters>(), It.IsAny<AuthConfig>(), It.IsAny<Progress<JSONMessage>>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
            dockerClient
                .Verify((x) => x.Containers.CreateContainerAsync(It.IsAny<CreateContainerParameters>(), It.IsAny<CancellationToken>()), Times.Exactly(1)); 
        }
        
        [Test]
        public async Task StartAsync_CreateDockerContainer_ShouldCreateAndStartContainer()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var jobConfiguration = new JobConfiguration
            {
                Image = "test:image",
                Environment = new List<string>()
            };
            var jobDetails = new Mock<JobDetails>();
            var cancellationToken = CancellationToken.None;
            var imagesCreateParameters = new Mock<ImagesCreateParameters>();
            var createContainerResponse = new CreateContainerResponse
            {
                ID = "test-docker-container-id"
            };
            var agentConfiguration = fixture.Create<IOptions<AgentConfiguration>>();
            var dockerJobHandlerServiceLogger = new Mock<ILogger<DockerJobHandlerService>>();
            var workerClient = new Mock<IWorkerClient>();
            var dockerClient = new Mock<IDockerClient>();

            var hubConnection = new Mock<MockHubConnection>();

            workerClient
                .SetupGet(x => x.HubConnection)
                .Returns(hubConnection.Object); 
            hubConnection.Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            dockerClient
                .Setup(x => x.Images.CreateImageAsync(imagesCreateParameters.Object, jobConfiguration.AuthConfig, new Progress<JSONMessage>(), cancellationToken))
                .Returns(Task.FromResult(createContainerResponse));
           dockerClient
                .Setup(x => x.Containers.CreateContainerAsync(It.IsAny<CreateContainerParameters>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(createContainerResponse));
           await using var dockerJobHandlerService = new DockerJobHandlerService(jobDetails.Object, jobConfiguration,workerClient.Object,agentConfiguration, dockerClient.Object, dockerJobHandlerServiceLogger.Object, cancellationToken);
            // Act
            await dockerJobHandlerService.StartAsync();
            
            // Assert
            dockerClient
                .Verify((x) => x.Images.CreateImageAsync(It.IsAny<ImagesCreateParameters>(), It.IsAny<AuthConfig>(), It.IsAny<Progress<JSONMessage>>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
            dockerClient
                .Verify((x) => x.Containers.CreateContainerAsync(It.IsAny<CreateContainerParameters>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
            dockerClient // TODO: place it in another test
                .Verify((x) => x.Containers.StartContainerAsync(It.IsAny<string>(), It.IsAny<ContainerStartParameters>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
    }
}