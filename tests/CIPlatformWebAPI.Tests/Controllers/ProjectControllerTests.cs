using System.Net;
using System.Threading.Tasks;
using Castle.Core.Logging;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Controllers;
using CIPlatformWebApi.DataTransfer.Project;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Services.Project;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.Controllers
{
    public class ProjectControllerTests
    {
        [Test]
        public async Task GetProjectsAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var projectService = new Mock<IProjectService>();
            var logger = new Mock<ILogger<ProjectController>>();
            
            var projectController = new ProjectController(projectService.Object, logger.Object);
            var expectedStatusCode = HttpStatusCode.OK;
            
            // Act
            var result = await projectController.GetProjectsAsync();
            var response = result as OkObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            projectService.Verify(x => x.GetProjectsAsync(), Times.Once);
        }
        
        [Test]
        public async Task GetProjectsAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var projectService = new Mock<IProjectService>();
            projectService.Setup(x => x.GetProjectsAsync())
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<ProjectController>>();
            
            var projectController = new ProjectController(projectService.Object, logger.Object);
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await projectController.GetProjectsAsync();
            var response = result as ObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            projectService.Verify(x => x.GetProjectsAsync(), Times.Once);
        }
        
        [Test]
        public async Task GetProjectAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var projectId = 0;
            var userEntity = new UserEntity();
            var projectDto = new ProjectDataDto();
            
            var projectService = new Mock<IProjectService>();
            projectService.Setup(x => x.GetProjectAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(projectDto));
            
            var logger = new Mock<ILogger<ProjectController>>();
            
            var projectController = new ProjectController(projectService.Object, logger.Object);
            projectController.ControllerContext = new Mock<ControllerContext>().Object;
            projectController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.OK;
            
            // Act
            var result = await projectController.GetProjectAsync(projectId);
            var response = result as OkObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            Assert.NotNull(response.Value);
            Assert.AreEqual(typeof(ProjectDataDto), response.Value.GetType());
            projectService.Verify(x => x.GetProjectAsync(It.IsAny<long>(), It.IsAny<UserEntity>()), Times.Once);
        }
        
        [Test]
        public async Task GetProjectAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var projectId = 0;
            var userEntity = new UserEntity();
            var projectService = new Mock<IProjectService>();
            projectService.Setup(x => x.GetProjectAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<ProjectController>>();
            
            var projectController = new ProjectController(projectService.Object, logger.Object);
            projectController.ControllerContext = new Mock<ControllerContext>().Object;
            projectController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await projectController.GetProjectAsync(projectId);
            var response = result as ObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            projectService.Verify(x => x.GetProjectAsync(It.IsAny<long>(), It.IsAny<UserEntity>()), Times.Once);
        }
        
        [Test]
        public async Task CreateProjectAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var userEntity = new UserEntity();
            var projectDto = new ProjectDto();
            var createProjectDto = new CreateProjectDto();
            
            var projectService = new Mock<IProjectService>();
            projectService.Setup(x => x.CreateProjectAsync(It.IsAny<CreateProjectDto>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(projectDto));
            
            var logger = new Mock<ILogger<ProjectController>>();
            
            var projectController = new ProjectController(projectService.Object, logger.Object);
            projectController.ControllerContext = new Mock<ControllerContext>().Object;
            projectController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.Created;
            
            // Act
            var result = await projectController.CreateProjectAsync(createProjectDto);
            var response = result as CreatedAtRouteResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            Assert.NotNull(response.Value);
            projectService.Verify(x => x.CreateProjectAsync(It.IsAny<CreateProjectDto>(), It.IsAny<UserEntity>()), Times.Once);
        }
        
        [Test]
        public async Task CreateProjectAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var userEntity = new UserEntity();
            var createProjectDto = new CreateProjectDto();
            
            var projectService = new Mock<IProjectService>();
            projectService.Setup(x => x.CreateProjectAsync(It.IsAny<CreateProjectDto>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<ProjectController>>();
            
            var projectController = new ProjectController(projectService.Object, logger.Object);
            projectController.ControllerContext = new Mock<ControllerContext>().Object;
            projectController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await projectController.CreateProjectAsync(createProjectDto);
            var response = result as ObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            Assert.NotNull(response.Value);
            projectService.Verify(x => x.CreateProjectAsync(It.IsAny<CreateProjectDto>(), It.IsAny<UserEntity>()), Times.Once);
        }
        
        [Test]
        public async Task UpdateProjectAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var projectId = 0;
            var userEntity = new UserEntity();
            var createProjectDto = new CreateProjectDto();
            
            var projectService = new Mock<IProjectService>();

            var logger = new Mock<ILogger<ProjectController>>();
            
            var projectController = new ProjectController(projectService.Object, logger.Object);
            projectController.ControllerContext = new Mock<ControllerContext>().Object;
            projectController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.OK;
            
            // Act
            var result = await projectController.UpdateProjectAsync(projectId, createProjectDto);
            var response = result as OkResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            projectService.Verify(x => x.UpdateProjectAsync(It.IsAny<long>(),It.IsAny<CreateProjectDto>(), It.IsAny<UserEntity>()), Times.Once);
        }
        
        [Test]
        public async Task UpdateProjectAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var projectId = 0;
            var userEntity = new UserEntity();
            var createProjectDto = new CreateProjectDto();
            
            var projectService = new Mock<IProjectService>();
            projectService.Setup(x => x.UpdateProjectAsync(It.IsAny<long>(),It.IsAny<CreateProjectDto>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<ProjectController>>();
            
            var projectController = new ProjectController(projectService.Object, logger.Object);
            projectController.ControllerContext = new Mock<ControllerContext>().Object;
            projectController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await projectController.UpdateProjectAsync(projectId, createProjectDto);
            var response = result as ObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            Assert.NotNull(response.Value);
            projectService.Verify(x => x.UpdateProjectAsync(It.IsAny<long>(), It.IsAny<CreateProjectDto>(), It.IsAny<UserEntity>()), Times.Once);
        }
        
        [Test]
        public async Task DeleteProjectAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var projectId = 0;
            var userEntity = new UserEntity();

            var projectService = new Mock<IProjectService>();

            var logger = new Mock<ILogger<ProjectController>>();
            
            var projectController = new ProjectController(projectService.Object, logger.Object);
            projectController.ControllerContext = new Mock<ControllerContext>().Object;
            projectController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.NoContent;
            
            // Act
            var result = await projectController.DeleteProjectAsync(projectId);
            var response = result as NoContentResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            projectService.Verify(x => x.DeleteProjectAsync(It.IsAny<long>() ,It.IsAny<UserEntity>()), Times.Once);
        }
        
        [Test]
        public async Task DeleteProjectAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var projectId = 0;
            var userEntity = new UserEntity();

            var projectService = new Mock<IProjectService>();
            projectService.Setup(x => x.DeleteProjectAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<ProjectController>>();
            
            var projectController = new ProjectController(projectService.Object, logger.Object);
            projectController.ControllerContext = new Mock<ControllerContext>().Object;
            projectController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await projectController.DeleteProjectAsync(projectId);
            var response = result as ObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            Assert.NotNull(response.Value);
            projectService.Verify(x => x.DeleteProjectAsync(It.IsAny<long>(), It.IsAny<UserEntity>()), Times.Once);
        }
    }
}