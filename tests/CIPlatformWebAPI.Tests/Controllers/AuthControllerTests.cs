using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Core.Logging;
using CIPlatformWebApi.Controllers;
using CIPlatformWebApi.DataTransfer.User;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Services.Auth;
using CIPlatformWebApi.Services.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Test]
        public async Task LoginAsync_InvalidLoginDetails_ShouldReturnErrorResponse()
        {
            // Arrange
            var loginDto = new LoginDto();
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<HttpContext>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));

            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var logger = new Mock<ILogger<AuthController>>();
            var authController = new AuthController(userService.Object, tokenService.Object, logger.Object);
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await authController.LoginAsync(loginDto);
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
        }
        
        [Test]
        public async Task LoginAsync_ValidLoginDetails_ShouldReturnValidResponse()
        {
            // Arrange
            var loginDto = new LoginDto();
            var authTokens = new AuthTokenDto
            {
                AccessToken = "abc-123",
                RefreshToken = "abc-123"
            };
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<HttpContext>()))
                .Returns(Task.FromResult(authTokens));

            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var logger = new Mock<ILogger<AuthController>>();
            var authController = new AuthController(userService.Object, tokenService.Object, logger.Object);
            var expectedStatusCode = HttpStatusCode.OK;
            
            // Act
            var result = await authController.LoginAsync(loginDto);
            var response = result as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            Assert.NotNull(response.Value);
            Assert.AreEqual(typeof(AuthTokenDto), response.Value.GetType());
        }
        
        [Test]
        public async Task LogoutAsync_InvalidHeader_ShouldReturnErrorResponse()
        {
            // Arrange
            var userService = new Mock<IUserService>();

            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            tokenService.Setup(x => x.VerifyRefreshTokenAsync(It.IsAny<string>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<AuthController>>();

            var authController = new AuthController(userService.Object, tokenService.Object, logger.Object);
            authController.ControllerContext = new Mock<ControllerContext>().Object;
            authController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers =
                    {
                        { "Authorization", "Bearer abc123" }
                    }
                }
            };

            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await authController.LogoutAsync();
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            tokenService.Verify(x => x.VerifyRefreshTokenAsync(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public async Task LogoutAsync_ValidHeader_ShouldReturnNoContentResponse()
        {
            // Arrange
            var user = "test";

            var claimPrincipal =
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Sub, user) }));
            var userService = new Mock<IUserService>();

            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            tokenService.Setup(x => x.VerifyRefreshTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(claimPrincipal));
            
            var logger = new Mock<ILogger<AuthController>>();

            var authController = new AuthController(userService.Object, tokenService.Object, logger.Object);
            authController.ControllerContext = new Mock<ControllerContext>().Object;
            authController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers =
                    {
                        { "Authorization", "Bearer abc123" }
                    }
                }
            };

            var expectedStatusCode = HttpStatusCode.NoContent;
            
            // Act
            var result = await authController.LogoutAsync();
            var response = result as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            tokenService.Verify(x => x.InvalidateAccessTokenAsync(It.IsAny<string>()), Times.Once);
            tokenService.Verify(x => x.InvalidRefreshTokenAsync(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public async Task LogoutAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var userService = new Mock<IUserService>();

            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();

            var logger = new Mock<ILogger<AuthController>>();

            var authController = new AuthController(userService.Object, tokenService.Object, logger.Object);
            authController.ControllerContext = new Mock<ControllerContext>().Object;

            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await authController.LogoutAsync();
            var response = result as UnauthorizedResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
        }
        
        [Test]
        public async Task RefreshTokenAsync_InvalidToken_ShouldReturnErrorResponse()
        {
            // Arrange
            var userService = new Mock<IUserService>();

            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            tokenService.Setup(x => x.VerifyRefreshTokenAsync(It.IsAny<string>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var logger = new Mock<ILogger<AuthController>>();

            var authController = new AuthController(userService.Object, tokenService.Object, logger.Object);
            authController.ControllerContext = new Mock<ControllerContext>().Object;
            authController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers =
                    {
                        { "Authorization", "Bearer abc123" }
                    }
                }
            };

            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await authController.RefreshTokenAsync();
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
        }
        
        [Test]
        public async Task RefreshTokenAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange

            var userService = new Mock<IUserService>();

            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();

            var logger = new Mock<ILogger<AuthController>>();

            var authController = new AuthController(userService.Object, tokenService.Object, logger.Object);
            authController.ControllerContext = new Mock<ControllerContext>().Object;

            var expectedStatusCode = HttpStatusCode.Unauthorized;
            
            // Act
            var result = await authController.RefreshTokenAsync();
            var response = result as UnauthorizedResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
        }
        
        [Test]
        public async Task RefreshTokenAsync_ValidToken_ShouldReturnValidResponse()
        {
            // Arrange
            var user = "test";
            var authTokens = new AuthTokenDto
            {
                AccessToken = "abc-123",
                RefreshToken = "abc-123"
            };

            var claimPrincipal =
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Sub, user) }));
            
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.GenerateAuthTokensAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(authTokens));

            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            tokenService.Setup(x => x.VerifyRefreshTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(claimPrincipal));
            
            var logger = new Mock<ILogger<AuthController>>();

            var authController = new AuthController(userService.Object, tokenService.Object, logger.Object);
            authController.ControllerContext = new Mock<ControllerContext>().Object;
            authController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers =
                    {
                        { "Authorization", "Bearer abc123" }
                    }
                }
            };

            var expectedStatusCode = HttpStatusCode.OK;
            
            // Act
            var result = await authController.RefreshTokenAsync();
            var response = result as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            Assert.NotNull(response.Value);
            Assert.AreEqual(typeof(AuthTokenDto), response.Value.GetType());
        }
    }
}