using System;
using System.Threading;
using AutoFixture;
using AutoFixture.AutoMoq;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using CodeManagerAgent.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CodeManagerAgent.Tests
{
    public class LinuxJobHandlerServiceFactoryTests
    {
        [Test]
        public void Create_NewLinuxJobHandlerServiceFactory_ShouldThrowArgumentNullException()
        {
            // Arrange
            Assert.Throws<ArgumentNullException>(() => new LinuxJobHandlerServiceFactory( null));
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
            Assert.DoesNotThrow(() =>  mockLinuxJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
            Assert.NotNull(mockLinuxJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
            Assert.IsInstanceOf<LinuxJobHandlerService>(mockLinuxJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
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
                mockLinuxJobHandlerServiceFactory.Create(null, null, cancellationToken));
        }
    }
}