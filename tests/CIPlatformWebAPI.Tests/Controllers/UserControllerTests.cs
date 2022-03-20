using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Castle.Core.Logging;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Controllers;
using CIPlatformWebApi.DataTransfer.User;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Services.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        [Test]
        public async Task GetUserByIdAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var userId = 0;
            var userDto = new UserDto();
            var userEntity = new UserEntity();
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.GetUserAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(userDto));
            
            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.OK;
            
            // Act
            var result = await userController.GetUserByIdAsync(userId);
            var response = result as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.GetUserAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetUserByIdAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var userId = 0;
            var userEntity = new UserEntity();
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.GetUserAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await userController.GetUserByIdAsync(userId);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.GetUserAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetUserAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var userId = 0;
            var userDto = new UserDto();
            var userEntity = new UserEntity();
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.GetUserAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(userDto));
            
            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.OK;
            
            // Act
            var result = await userController.GetUserAsync();
            var response = result as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.GetUserAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var userId = 0;
            var userEntity = new UserEntity();
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.GetUserAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await userController.GetUserAsync();
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.GetUserAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task UpdateUserAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var userId = 0;
            var updateUserDto = new UpdateUserDto();
            var userEntity = new UserEntity();
            
            var userService = new Mock<IUserService>();

            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.NoContent;
            
            // Act
            var result = await userController.UpdateUserAsync(userId, updateUserDto);
            var response = result as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.UpdateUserAsync(It.IsAny<long>(), It.IsAny<UpdateUserDto>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task UpdateUserAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var userId = 0;
            var updateUserDto = new UpdateUserDto();
            var userEntity = new UserEntity();
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.UpdateUserAsync(It.IsAny<long>(), It.IsAny<UpdateUserDto>(),It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await userController.UpdateUserAsync(userId, updateUserDto);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.UpdateUserAsync(It.IsAny<long>(), It.IsAny<UpdateUserDto>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task DeleteUserAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var userId = 0;
            var userEntity = new UserEntity();
            
            var userService = new Mock<IUserService>();

            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.NoContent;
            
            // Act
            var result = await userController.DeleteUserAsync(userId);
            var response = result as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.DeleteUserAsync(It.IsAny<long>(),It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task DeleteUserAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var userId = 0;
            var userEntity = new UserEntity();
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.DeleteUserAsync(It.IsAny<long>(),It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await userController.DeleteUserAsync(userId);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.DeleteUserAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetUsersAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var userEntity = new UserEntity();
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.GetUsersAsync(It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(new List<UserDto>() as IEnumerable<UserDto>));

            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.OK;
            
            // Act
            var result = await userController.GetUsersAsync();
            var response = result as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.GetUsersAsync(It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetUsersAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var userEntity = new UserEntity();
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.GetUsersAsync(It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await userController.GetUsersAsync();
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.GetUsersAsync(It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task RegisterAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var userEntity = new UserEntity();
            var createUserDto = new CreateUserDto();
            
            var userService = new Mock<IUserService>();

            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Created;
            
            // Act
            var result = await userController.RegisterAsync(createUserDto);
            var response = result as StatusCodeResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.CreateUserAsync(It.IsAny<CreateUserDto>()));
        }
        
        [Test]
        public async Task RegisterAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var userEntity = new UserEntity();
            var createUserDto = new CreateUserDto();
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserDto>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<UserController>>();
            
            var userController = new UserController(userService.Object, logger.Object);
            userController.ControllerContext = new Mock<ControllerContext>().Object;
            userController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await userController.RegisterAsync(createUserDto);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            userService.Verify(x => x.CreateUserAsync(It.IsAny<CreateUserDto>()));
        }
    }
}