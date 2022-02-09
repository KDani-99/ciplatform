﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManager.Data.Extensions;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Configuration;
using CodeManagerWebApi.Exceptions;
using CodeManagerWebApi.Extensions;
using CodeManagerWebApi.Utils.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CodeManagerWebApi.Services
{
    public class UserService : IUserService
    {
        private readonly ICredentialManagerService _credentialManagerService;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IUserRepository _userRepository;

        public UserService(
            IUserRepository userRepository,
            ITokenService<JwtSecurityToken> tokenService,
            ICredentialManagerService credentialManagerService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _credentialManagerService = credentialManagerService ??
                                        throw new ArgumentNullException(nameof(credentialManagerService));
        }

        public async Task CreateUser(CreateUserDto createUserDto)
        {
            if (await _userRepository.GetByUsernameAsync(createUserDto.Username) is not null)
                throw new UsernameTakenException();

            if (await _userRepository.GetByEmailAsync(createUserDto.Email) is not null)
                throw new EmailAlreadyInUseException();

            var user = new User
            {
                Username = createUserDto.Username,
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = _credentialManagerService.CreateHashedPassword(createUserDto.Password),
                Roles = new [] {Roles.User},
                RegistrationTimestamp = DateTime.Now,
            };
            
            await _userRepository.CreateAsync(user);
        }

        public async Task<UserDto> GetUserAsync(long id, User user)
        {
            if (user.Id != id && user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to perform this action.");
            }
            
            var selectedUser = await _userRepository.GetAsync(id);
            
            return new UserDto
            {
                Id = selectedUser.Id,
                Username = selectedUser.Username,
                Name = selectedUser.Name,
                Email = selectedUser.Email,
                IsAdmin = selectedUser.IsAdmin(),
                Teams = selectedUser.Teams.Count,
                Registration = selectedUser.RegistrationTimestamp
            };
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync(User user)
        {
            var users = await _userRepository.GetAsync((dbUser) => dbUser.Id != user.Id);

            return users.Select(dbUser =>  new UserDto
            {
                Id = dbUser.Id,
                Username = dbUser.Username,
                Name = dbUser.Name,
                Email = dbUser.Email,
                IsAdmin = dbUser.IsAdmin(),
                Teams = dbUser.Teams.Count,
                Registration = dbUser.RegistrationTimestamp
            });
        }

        public Task<bool> ExistsAsync(long id)
        {
            // TODO: throw exception if it does not exist
            return _userRepository.ExistsAsync(user => user.Id == id);
        }

        public async Task<AuthTokenDto> LoginAsync(LoginDto loginDto, HttpContext httpContext)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            if (user is null) throw new InvalidCredentialsException();

            if (!_credentialManagerService.VerifyPassword(loginDto.Password, user.Password))
                throw new InvalidCredentialsException();

            var accessToken = await _tokenService.CreateAccessTokenAsync(user);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);
            
            user.RefreshTokenSignature = refreshToken.Id; // TODO -> remove from database

            await _userRepository.UpdateAsync(user);

            return new AuthTokenDto
            {
                AccessToken = accessToken.ToBase64String(),
                RefreshToken = refreshToken.ToBase64String()
            };
        }

        public async Task<AuthTokenDto> GenerateAuthTokensAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user is null) throw new UserDoesNotExistException();
            
            var accessToken = await _tokenService.CreateAccessTokenAsync(user);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);
            
            return new AuthTokenDto
            {
                AccessToken = accessToken.ToBase64String(),
                RefreshToken = refreshToken.ToBase64String()
            };
        }

        public async Task UpdateUserAsync(long id, UpdateUserDto updateUserDto, User user)
        {
            var userToUpdate = await _userRepository.GetAsync(id) ?? throw new UserDoesNotExistException();

            userToUpdate.Email = updateUserDto.Email;
            userToUpdate.Name = updateUserDto.Name;
            userToUpdate.Username = updateUserDto.Username;
            if (updateUserDto.Password != null)
            {
                userToUpdate.Password = _credentialManagerService.CreateHashedPassword(updateUserDto.Password);  
            }

            userToUpdate.Roles = updateUserDto.IsAdmin ? new[] {Roles.User, Roles.Admin} : new[] {Roles.User};

            await _userRepository.UpdateAsync(userToUpdate);
        }

        public async Task DeleteUserAsync(long id, User user)
        {
            var userToDelete = await _userRepository.GetAsync(id) ?? throw new UserDoesNotExistException();

            if (userToDelete.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to delete admin users");
            }

            if (userToDelete.Id == user.Id)
            {
                throw new UnauthorizedAccessWebException("You can not delete yourself.");
            }

            await _userRepository.DeleteAsync(id);
        }
    }
}