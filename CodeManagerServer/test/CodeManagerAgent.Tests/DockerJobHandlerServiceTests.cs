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
        [SetUp]
        public void Setup()
        {

        }

        interface IHubConnectionProxy
        {
            
        }

        public class Test : HubConnection
        {
            public Test(IConnectionFactory connectionFactory, IHubProtocol hubProtocol, EndPoint endpoint, IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : base(connectionFactory, hubProtocol, endpoint, serviceProvider, loggerFactory) {}
        }

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
            var webSocketConfiguration = new WebSocketConfiguration
            {
                Host = "http://test-host:5918",
                Hub = "test"
            };
            var jobDetails = new Mock<JobDetails>();
            var cancellationToken = CancellationToken.None;
            var createContainerParameters = new CreateContainerParameters
            {
                Image = jobConfiguration.Image,
                Env = jobConfiguration.Environment,
                AttachStdout = true,
                AttachStderr = true,
                Tty = true
            };
            var imagesCreateParameters = new Mock<ImagesCreateParameters>();
            var createContainerResponse = new CreateContainerResponse
            {
                ID = "test-docker-container-id"
            };
            var agentConfiguration = fixture.Create<IOptions<AgentConfiguration>>();
            var dockerJobHandlerServiceLogger = new Mock<ILogger<DockerJobHandlerService>>();
            var serviceProvider = fixture.Create<IServiceProvider>();
            var logger = fixture.Create<ILogger<WorkerClient>>();
            var workerClient = new Mock<IWorkerClient>();
            var dockerClient = new Mock<IDockerClient>();
            
            var test = new Mock<Test>();

            workerClient
                .SetupGet(x => x.HubConnection)
                .Returns(test.Object); 
            test.Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            dockerClient
                .Setup(x => x.Images.CreateImageAsync(imagesCreateParameters.Object, jobConfiguration.AuthConfig, new Progress<JSONMessage>(), cancellationToken))
                .Returns(Task.FromResult(createContainerResponse));
           dockerClient
                .Setup(x => x.Containers.CreateContainerAsync(It.IsAny<CreateContainerParameters>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(createContainerResponse));
            var dockerJobHandlerService = new DockerJobHandlerService(jobDetails.Object, jobConfiguration,workerClient.Object,agentConfiguration, dockerClient.Object, dockerJobHandlerServiceLogger.Object, cancellationToken);
            // Act
            await dockerJobHandlerService.StartAsync();
            // Assert
            dockerClient.Verify((x) => x.Containers.CreateContainerAsync(createContainerParameters, cancellationToken), Times.Exactly(1));
        }
    }
}