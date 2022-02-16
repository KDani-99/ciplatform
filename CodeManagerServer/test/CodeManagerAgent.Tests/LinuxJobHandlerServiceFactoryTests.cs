using System;
using System.Threading;
using AutoFixture;
using AutoFixture.AutoMoq;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Factories;
using CIPlatformWorker.Services;
using CIPlatform.Data.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace CIPlatformWorker.Tests
{
    public class LinuxJobHandlerServiceFactoryTests
    {
        [Test]
        public void Create_NewLinuxJobHandlerServiceFactory_ShouldThrowArgumentNullException()
        {
            // Arrange
            Assert.Throws<ArgumentNullException>(() => new LinuxJobHandlerServiceFactory(null));
        }

        [Test]
        public void Create_NewLinuxJobHandlerServiceWithCreate_ShouldReturnNewLinuxHandlerServiceInstance()
        {
            // Arrange
            var jobConfiguration = new JobConfiguration();
            var jobDetails = new JobDetails();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var agentConfiguration = fixture.Create<IOptions<AgentConfiguration>>();
            var cancellationToken = CancellationToken.None;
            var mockLinuxJobHandlerServiceFactory = new LinuxJobHandlerServiceFactory(agentConfiguration);

            // Act and Assert
            Assert.DoesNotThrow(
                () => mockLinuxJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
            Assert.NotNull(mockLinuxJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
            Assert.IsInstanceOf<LinuxJobHandlerService>(
                mockLinuxJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
        }

        [Test]
        public void Create_NewLinuxJobHandlerServiceWithCreate_ShouldThrowArgumentNullException()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var agentConfiguration = fixture.Create<IOptions<AgentConfiguration>>();
            var cancellationToken = CancellationToken.None;
            var mockLinuxJobHandlerServiceFactory = new LinuxJobHandlerServiceFactory(agentConfiguration);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() =>
                                                     mockLinuxJobHandlerServiceFactory.Create(
                                                         null, null, cancellationToken));
        }
    }
}