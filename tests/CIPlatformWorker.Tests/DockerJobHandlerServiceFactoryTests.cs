using System;
using System.Threading;
using AutoFixture;
using AutoFixture.AutoMoq;
using CIPlatform.Data.Configuration;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Factories.Job;
using CIPlatformWorker.Services.Job;
using Docker.DotNet;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CIPlatformWorker.Tests
{
 public class DockerJobHandlerServiceFactoryTests
    {
        [Test]
        public void Create_NewDockerJobHandlerServiceFactory_ShouldThrowArgumentNullException()
        {
            // Arrange
            Assert.Throws<ArgumentNullException>(() => new DockerJobHandlerServiceFactory(null, null));
        }

        [Test]
        public void Create_NewDockerJobHandlerServiceWithCreate_ShouldReturnNewDockerHandlerServiceInstance()
        {
            // Arrange
            var jobConfiguration = new JobConfiguration();
            var jobDetails = new JobDetails();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var agentConfiguration = fixture.Create<IOptions<WorkerConfiguration>>();
            var dockerClient = new Mock<IDockerClient>();
            var cancellationToken = CancellationToken.None;
            var mockDockerJobHandlerServiceFactory =
                new DockerJobHandlerServiceFactory(dockerClient.Object, agentConfiguration);

            // Act and Assert
            Assert.DoesNotThrow(
                () => mockDockerJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
            Assert.NotNull(mockDockerJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
            Assert.IsInstanceOf<DockerJobHandlerService>(
                mockDockerJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
        }

        [Test]
        public void Create_NewDockerJobHandlerServiceWithCreate_ShouldThrowArgumentNullException()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var agentConfiguration = fixture.Create<IOptions<WorkerConfiguration>>();
            var dockerClient = new Mock<IDockerClient>();
            var cancellationToken = CancellationToken.None;
            var mockDockerJobHandlerServiceFactory =
                new DockerJobHandlerServiceFactory(dockerClient.Object, agentConfiguration);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() =>
                                                     mockDockerJobHandlerServiceFactory.Create(
                                                         null, null, cancellationToken));
        }
    }
}