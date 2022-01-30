using AutoFixture;
using AutoFixture.AutoMoq;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using CodeManagerAgent.WebSocket;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CodeManagerAgent.Tests
{
    public class DockerJobHandlerServiceFactoryTests
    {
        [Test]
        public void CreateShouldReturnNewObject()
        {
            // Arrange
            var repository = "test_repository";
            var token = "test_token";
            var jobConfiguration = new JobConfiguration();
            
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var returnValue = fixture.Create<DockerJobHandlerService>();
            
            var mockDockerJobHandlerServiceFactory = new Mock<DockerJobHandlerServiceFactory>();
            mockDockerJobHandlerServiceFactory
                .Setup(x => x.Create(repository, token, jobConfiguration, default))
                .Returns(returnValue);
            // Act
            var instance =
                mockDockerJobHandlerServiceFactory.Object.Create(repository, token, jobConfiguration, default);
            // Assert
            Assert.NotNull(instance);
        }
    }
}