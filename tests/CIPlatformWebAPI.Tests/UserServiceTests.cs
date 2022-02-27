using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformWebApi.DataTransfer.User;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Services.Auth;
using CIPlatformWebApi.Services.User;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests
{
    public class UserServiceTests
    {
        [Test]
        public void CreateUserAsync_ValidData_ShouldSaveEntity()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();
            var createUserDto = new CreateUserDto
            {
                Username = "test",
                Name = "John Test",
                Email = "test@email.com",
                Password = "secreTpassword123"
            };
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.DoesNotThrowAsync(() => userService.CreateUserAsync(createUserDto));
            userRepository.Verify(x => x.CreateAsync(It.IsAny<UserEntity>()), Times.Once);
        }
        
        [Test]
        public void CreateUserAsync_UserExists_ShouldThrowUsernameTakenException()
        {
            // Arrange
            var existingUser = new UserEntity();
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(existingUser));
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();
            var createUserDto = new CreateUserDto
            {
                Username = "test",
                Name = "John Test",
                Email = "test@email.com",
                Password = "secreTpassword123"
            };
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.ThrowsAsync<UsernameTakenException>(() => userService.CreateUserAsync(createUserDto));
        }
        
        [Test]
        public void CreateUserAsync_UserExists_ShouldThrowEmailAlreadyInUseException()
        {
            // Arrange
            var existingUser = new UserEntity();
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(existingUser));
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();
            var createUserDto = new CreateUserDto
            {
                Username = "test",
                Name = "John Test",
                Email = "test@email.com",
                Password = "secreTpassword123"
            };
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.ThrowsAsync<EmailAlreadyInUseException>(() => userService.CreateUserAsync(createUserDto));
        }
        
        [Test]
        public void GetUserAsync_UserExists_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var userId = 0;
            var existingUser = new UserEntity
            {
                Id = 1,
                Roles = new[] { Roles.User }
            };
            var userRepository = new Mock<IUserRepository>();
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();

            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => userService.GetUserAsync(userId, existingUser));
        }
        
        [Test]
        public async Task GetUserAsync_UserExists_ShouldReturnValidUserDto()
        {
            // Arrange
            var userId = 0;
            var existingUser = new UserEntity
            {
                Id = 1,
                Roles = new[] { Roles.Admin }
            };
            var dbUser = new UserEntity
            {
                Id = userId,
                Username = "test",
                Roles = new[] { Roles.User },
                Name = "John Test",
                Email = "test@email.com",
                Teams = new List<TeamEntity>(),
                RegistrationTimestamp = DateTime.Now
            };
            var expectedDto = new UserDto
            {
                Id = dbUser.Id,
                Username = dbUser.Username,
                Name = dbUser.Name,
                Email = dbUser.Email,
                IsAdmin = false,
                Teams = 0,
                Registration = dbUser.RegistrationTimestamp
            };
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(dbUser));
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();

            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);
            
            // Act
            var userDto = await userService.GetUserAsync(userId, existingUser);
                
            // Assert
            userRepository.Verify(x => x.GetAsync(It.IsAny<long>()), Times.Once);
            Assert.AreEqual(expectedDto, userDto);
        }

        [Test]
        public async Task GetUsersAsync_UserExists_ShouldReturnValidUserDto()
        {
            // Arrange
            var existingUser = new UserEntity
            {
                Id = 1,
                Roles = new[] { Roles.User }
            };
            var expectedResult = new List<UserEntity>();
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(Task.FromResult(expectedResult));
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();

            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act
            var users = await userService.GetUsersAsync(existingUser);
            
            // Assert
            Assert.AreEqual(expectedResult.Count, users.Count());
        }
        
        [Test]
        public void LoginAsync_InvalidCredentials_ShouldThrowInvalidCredentialsException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "test",
                Password = "testpassword123"
            };
            var httpContext = new Mock<HttpContext>();
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(null as UserEntity));
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();

            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => userService.LoginAsync(loginDto, httpContext.Object));
        }
        
        [Test]
        public void LoginAsync_InvalidPassword_ShouldThrowInvalidCredentialsException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "test",
                Password = "testpassword123"
            };
            var userEntity = new UserEntity
            {
                Id = 1
            };
            var httpContext = new Mock<HttpContext>();
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(userEntity));
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();
            credentialManagerService.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => userService.LoginAsync(loginDto, httpContext.Object));
        }
        
        [Test]
        public async Task LoginAsync_ValidCredentials_ShouldCreateAuthTokenDto()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "test",
                Password = "testpassword123"
            };
            var userEntity = new UserEntity
            {
                Id = 1
            };
            var httpContext = new Mock<HttpContext>();
            var userRepository = new Mock<IUserRepository>();
            var jwtToken = new JwtSecurityToken();
            userRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(userEntity));
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            
            tokenService.Setup(x => x.CreateAccessTokenAsync(It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(jwtToken));
            tokenService.Setup(x => x.CreateRefreshTokenAsync(It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(jwtToken));
            
            var credentialManagerService = new Mock<ICredentialManagerService>();
            credentialManagerService.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act
            var result = await userService.LoginAsync(loginDto, httpContext.Object);
            
            // Assert
            userRepository.Verify(x => x.UpdateAsync(It.IsAny<UserEntity>()), Times.Once);
            tokenService.Verify(x => x.CreateAccessTokenAsync(It.IsAny<UserEntity>()));
            tokenService.Verify(x => x.CreateRefreshTokenAsync(It.IsAny<UserEntity>()));
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
        }
        
        [Test]
        public void GenerateAuthTokensAsync_InvalidUsername_ShouldThrowUserDoesNotExistException()
        {
            // Arrange
            var username = "test-user";
            var jwtToken = new JwtSecurityToken();
            var userRepository = new Mock<IUserRepository>();
           
            userRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(null as UserEntity));
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            
            tokenService.Setup(x => x.CreateAccessTokenAsync(It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(jwtToken));
            tokenService.Setup(x => x.CreateRefreshTokenAsync(It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(jwtToken));
            
            var credentialManagerService = new Mock<ICredentialManagerService>();
            credentialManagerService.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.ThrowsAsync<UserDoesNotExistException>(() =>
                userService.GenerateAuthTokensAsync(username));
        }
        
        [Test]
        public async Task GenerateTokensAsync_ValidCredentials_ShouldCreateAuthTokenDto()
        {
            // Arrange
            var username = "test-user";
            var userEntity = new UserEntity
            {
                Id = 1,
                Username = username
            };
            
            var userRepository = new Mock<IUserRepository>();
            var jwtToken = new JwtSecurityToken();
            userRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(userEntity));
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            
            tokenService.Setup(x => x.CreateAccessTokenAsync(It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(jwtToken));
            tokenService.Setup(x => x.CreateRefreshTokenAsync(It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(jwtToken));
            
            var credentialManagerService = new Mock<ICredentialManagerService>();

            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act
            var result = await userService.GenerateAuthTokensAsync(username);
            
            // Assert
            tokenService.Verify(x => x.CreateAccessTokenAsync(It.IsAny<UserEntity>()));
            tokenService.Verify(x => x.CreateRefreshTokenAsync(It.IsAny<UserEntity>()));
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
        }
        
        [Test]
        public void UpdateUserAsync_InvalidId_ShouldThrowUserDoesNotExistException()
        {
            // Arrange
            var userId = 0;
            var updateUserDto = new UpdateUserDto();
            var userEntity = new UserEntity();

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(null as UserEntity));
            
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.ThrowsAsync<UserDoesNotExistException>(() => userService.UpdateUserAsync(userId, updateUserDto, userEntity));
        }
        
        [Test]
        public void UpdateUserAsync_ValidUpdateDto_ShouldUpdateUser()
        {
            // Arrange
            var userId = 0;
            var updateUserDto = new UpdateUserDto
            {
                Username = "new-username",
                Name = "new-name",
                Email = "new-email",
                Password = "new-password",
                IsAdmin = true
            };
            var expectedRoles = new[] { Roles.User, Roles.Admin };
            var userEntity = new UserEntity();
            
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(userEntity));
            
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();
            credentialManagerService.Setup(x => x.CreateHashedPassword(It.IsAny<string>()))
                .Returns(updateUserDto.Password);
            
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.DoesNotThrowAsync(() => userService.UpdateUserAsync(userId, updateUserDto, userEntity));
            userRepository.Verify(x => x.UpdateAsync(It.IsAny<UserEntity>()), Times.Once);
            Assert.AreEqual(updateUserDto.Username, userEntity.Username);
            Assert.AreEqual(updateUserDto.Name, userEntity.Name);
            Assert.AreEqual(updateUserDto.Email, userEntity.Email);
            Assert.AreEqual(updateUserDto.Password, userEntity.Password);
            Assert.AreEqual(updateUserDto.Password, userEntity.Password);
            Assert.AreEqual(expectedRoles, userEntity.Roles);
        }
        
        [Test]
        public void DeleteUserAsync_InvalidId_ShouldThrowUserDoesNotExistException()
        {
            // Arrange
            var userId = 0;
            var userEntity = new UserEntity();

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(null as UserEntity));
            
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.ThrowsAsync<UserDoesNotExistException>(() => userService.DeleteUserAsync(userId, userEntity));
        }
        
        [Test]
        public void DeleteUserAsync_AdminUser_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var userId = 0;
            var userEntity = new UserEntity
            {
                Roles = new[] { Roles.Admin }
            };

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(userEntity));
            
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => userService.DeleteUserAsync(userId, userEntity));
        }
        
        [Test]
        public void DeleteUserAsync_SameUser_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var userId = 0;
            var userEntity = new UserEntity
            {
                Id = userId,
                Roles = new[] { Roles.User }
            };

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(userEntity));
            
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => userService.DeleteUserAsync(userId, userEntity));
        }
        
        [Test]
        public void DeleteUserAsync_ValidId_ShouldDeleteUser()
        {
            // Arrange
            var userId = 0;
            var userEntity = new UserEntity();

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(null as UserEntity));
            
            var tokenService = new Mock<ITokenService<JwtSecurityToken>>();
            var credentialManagerService = new Mock<ICredentialManagerService>();
            var userService =
                new UserService(userRepository.Object, tokenService.Object, credentialManagerService.Object);

            // Act and Assert
            Assert.DoesNotThrowAsync(() => userService.DeleteUserAsync(userId, userEntity));
            userRepository.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.Once);
        }
    }
}