using System;
using CIPlatformWebApi.Services.Project;
using CIPlatformWebApi.Services.Run;
using CIPlatformWebApi.Strategies;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.WebSocket.ConnectionHandlers
{
    public class ResultChannelConnectionHandlerFactoryTests
    {
        [Test]
        public void Create_ProjectType_ShouldReturnNewProjectResultChannelConnectionHandler()
        {
            // Arrange
            var type = "project";
            var projectService = new Mock<IProjectService>();
            var runService = new Mock<IRunService>();
            
            var resultChannelConnectionHandlerFactory = new ResultChannelConnectionHandlerFactory(projectService.Object, runService.Object);

            // Act
            var result = resultChannelConnectionHandlerFactory.Create(type);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(typeof(ProjectResultChannelConnectionHandler), result.GetType());
        }
        
        [Test]
        public void Create_RunType_ShouldReturnNewRunResultChannelConnectionHandler()
        {
            // Arrange
            var type = "run";
            var projectService = new Mock<IProjectService>();
            var runService = new Mock<IRunService>();
            
            var resultChannelConnectionHandlerFactory = new ResultChannelConnectionHandlerFactory(projectService.Object, runService.Object);

            // Act
            var result = resultChannelConnectionHandlerFactory.Create(type);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(typeof(RunResultChannelConnectionHandler), result.GetType());
        }
        
        [Test]
        public void Create_JobType_ShouldReturnNewJobResultChannelConnectionHandler()
        {
            // Arrange
            var type = "job";
            var projectService = new Mock<IProjectService>();
            var runService = new Mock<IRunService>();
            
            var resultChannelConnectionHandlerFactory = new ResultChannelConnectionHandlerFactory(projectService.Object, runService.Object);

            // Act
            var result = resultChannelConnectionHandlerFactory.Create(type);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(typeof(JobResultChannelConnectionHandler), result.GetType());
        }
        
        [Test]
        public void Create_StepType_ShouldReturnNewSteoResultChannelConnectionHandler()
        {
            // Arrange
            var type = "step";
            var projectService = new Mock<IProjectService>();
            var runService = new Mock<IRunService>();
            
            var resultChannelConnectionHandlerFactory = new ResultChannelConnectionHandlerFactory(projectService.Object, runService.Object);

            // Act
            var result = resultChannelConnectionHandlerFactory.Create(type);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(typeof(StepResultChannelConnectionHandler), result.GetType());
        }
        
        [Test]
        public void Create_InvalidType_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var type = "invalid";
            var projectService = new Mock<IProjectService>();
            var runService = new Mock<IRunService>();
            
            var resultChannelConnectionHandlerFactory = new ResultChannelConnectionHandlerFactory(projectService.Object, runService.Object);
            
            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => resultChannelConnectionHandlerFactory.Create(type));
        }
    }
}