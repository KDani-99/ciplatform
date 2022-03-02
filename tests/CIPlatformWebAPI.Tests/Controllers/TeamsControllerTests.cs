using System.Net;
using System.Threading.Tasks;
using Castle.Core.Logging;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Controllers;
using CIPlatformWebApi.DataTransfer.Team;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Services.Team;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.Controllers
{
    public class TeamControllerTests
    {
        [Test]
        public async Task GetTeamsAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var userEntity = new UserEntity();
            var teamService = new Mock<ITeamService>();
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.OK;

            // Act
            var result = await teamsController.GetTeamsAsync();
            var response = result as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.GetTeamsAsync(It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetTeamsAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var userEntity = new UserEntity();
            
            var teamService = new Mock<ITeamService>();
            teamService.Setup(x => x.GetTeamsAsync(It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await teamsController.GetTeamsAsync();
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.GetTeamsAsync(It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetTeamAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var teamService = new Mock<ITeamService>();
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.OK;

            // Act
            var result = await teamsController.GetTeamAsync(teamId);
            var response = result as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.GetTeamAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetTeamAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            
            var teamService = new Mock<ITeamService>();
            teamService.Setup(x => x.GetTeamAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await teamsController.GetTeamAsync(teamId);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.GetTeamAsync(It.IsAny<long>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task CreateTeamAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var createTeamDto = new TeamDto();
            var userEntity = new UserEntity();

            var teamService = new Mock<ITeamService>();
            teamService.Setup(x => x.CreateTeamAsync(It.IsAny<TeamDto>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(createTeamDto));
            
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Created;

            // Act
            var result = await teamsController.CreateTeamAsync(createTeamDto);
            var response = result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.CreateTeamAsync(It.IsAny<TeamDto>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task CreateTeamAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var userEntity = new UserEntity();
            var createTeamDto = new TeamDto();
            
            var teamService = new Mock<ITeamService>();
            teamService.Setup(x => x.CreateTeamAsync(It.IsAny<TeamDto>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await teamsController.CreateTeamAsync(createTeamDto);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.CreateTeamAsync(It.IsAny<TeamDto>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task UpdateTeamAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var teamId = 0;
            var updateTeamDto = new TeamDto();
            var userEntity = new UserEntity();

            var teamService = new Mock<ITeamService>();

            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.OK;

            // Act
            var result = await teamsController.UpdateTeamAsync(teamId, updateTeamDto);
            var response = result as OkResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.UpdateTeamAsync(It.IsAny<TeamDto>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task UpdateTeamAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var createTeamDto = new TeamDto();
            
            var teamService = new Mock<ITeamService>();
            teamService.Setup(x => x.UpdateTeamAsync(It.IsAny<TeamDto>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await teamsController.UpdateTeamAsync(teamId, createTeamDto);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.UpdateTeamAsync(It.IsAny<TeamDto>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task DeleteTeamAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();

            var teamService = new Mock<ITeamService>();

            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.NoContent;

            // Act
            var result = await teamsController.DeleteTeamAsync(teamId);
            var response = result as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.DeleteTeamAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task DeleteTeamAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();

            var teamService = new Mock<ITeamService>();
            teamService.Setup(x => x.DeleteTeamAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await teamsController.DeleteTeamAsync(teamId);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.DeleteTeamAsync(It.IsAny<long>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task JoinAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();

            var teamService = new Mock<ITeamService>();

            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.NoContent;

            // Act
            var result = await teamsController.JoinAsync(teamId);
            var response = result as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.JoinAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task JoinAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();

            var teamService = new Mock<ITeamService>();
            teamService.Setup(x => x.JoinAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await teamsController.JoinAsync(teamId);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.JoinAsync(It.IsAny<long>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task KickAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var kickMemberDto = new KickMemberDto();

            var teamService = new Mock<ITeamService>();

            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.NoContent;

            // Act
            var result = await teamsController.KickAsync(teamId, kickMemberDto);
            var response = result as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.KickMemberAsync(It.IsAny<long>(), It.IsAny<long>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task KickAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var kickMemberDto = new KickMemberDto();

            var teamService = new Mock<ITeamService>();
            teamService.Setup(x => x.KickMemberAsync(It.IsAny<long>(),It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await teamsController.KickAsync(teamId, kickMemberDto);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.KickMemberAsync(It.IsAny<long>(),It.IsAny<long>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task UpdateRoleAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var updateRoleDto = new UpdateRoleDto();

            var teamService = new Mock<ITeamService>();

            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.NoContent;

            // Act
            var result = await teamsController.UpdateRoleAsync(teamId, updateRoleDto);
            var response = result as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.UpdateRoleAsync(It.IsAny<long>(), It.IsAny<UpdateRoleDto>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task UpdateRoleAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var updateRoleAsync = new UpdateRoleDto();

            var teamService = new Mock<ITeamService>();
            teamService.Setup(x => x.UpdateRoleAsync(It.IsAny<long>(),It.IsAny<UpdateRoleDto>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await teamsController.UpdateRoleAsync(teamId, updateRoleAsync);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.UpdateRoleAsync(It.IsAny<long>(),It.IsAny<UpdateRoleDto>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task AddMemberAsyncRoleAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var addMemberDto = new AddMemberDto();

            var teamService = new Mock<ITeamService>();

            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.NoContent;

            // Act
            var result = await teamsController.AddMemberAsync(teamId, addMemberDto);
            var response = result as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.AddMemberAsync(It.IsAny<long>(), It.IsAny<AddMemberDto>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task AddMemberAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var addMemberDto = new AddMemberDto();

            var teamService = new Mock<ITeamService>();
            teamService.Setup(x => x.AddMemberAsync(It.IsAny<long>(),It.IsAny<AddMemberDto>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<TeamsController>>();
            
            var teamsController = new TeamsController(teamService.Object, logger.Object);
            teamsController.ControllerContext = new Mock<ControllerContext>().Object;
            teamsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await teamsController.AddMemberAsync(teamId, addMemberDto);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            teamService.Verify(x => x.AddMemberAsync(It.IsAny<long>(),It.IsAny<AddMemberDto>(),It.IsAny<UserEntity>()));
        }
    }
}