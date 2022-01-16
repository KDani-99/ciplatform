﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Entities.Configuration;
using CodeManagerWebApi.Exceptions;
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
        private readonly IPlanRepository _planRepository;
        private readonly UserConfiguration _userConfiguration;

        public UserService(
            IUserRepository userRepository,
            IPlanRepository planRepository,
            ITokenService<JwtSecurityToken> tokenService,
            ICredentialManagerService credentialManagerService,
            IOptions<UserConfiguration> userConfiguration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _credentialManagerService = credentialManagerService ??
                                        throw new ArgumentNullException(nameof(credentialManagerService));
            _userConfiguration = userConfiguration?.Value ?? throw new ArgumentNullException(nameof(userConfiguration));
        }

        public async Task CreateUser(UserDto userDto)
        {
            if (await _userRepository.GetByUsernameAsync(userDto.Username) is not null)
                throw new UsernameTakenException();

            if (await _userRepository.GetByEmailAsync(userDto.Email) is not null)
                throw new EmailAlreadyInUseException();

            var defaultPlan = (await _planRepository.GetAsync(plan => plan.Name == _userConfiguration.DefaultPlan)).FirstOrDefault();

            var user = new User
            {
                Username = userDto.Username,
                Name = userDto.Name,
                Email = userDto.Email,
                Image = null,
                Password = _credentialManagerService.CreateHashedPassword(userDto.Password),
                Roles = new [] {Roles.User},
                IsActive = false,
                RegistrationTimestamp = DateTime.Now,
                Plan = defaultPlan
            };
            
            await _userRepository.CreateAsync(user);
        }

        public async Task<UserDto> GetUserAsync(long id)
        {
            return (await _userRepository.GetAsync(id)).FromUser();
        }

        public Task<bool> ExistsAsync(long id)
        {
            return _userRepository.ExistsAsync(user => user.Id == id);
        }

        public async Task<AuthTokenDto> LoginAsync(LoginDto loginDto, HttpContext httpContext)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            if (user is null) throw new UserDoesNotExistException();

            if (!_credentialManagerService.VerifyPassword(loginDto.Password, user.Password))
                throw new InvalidCredentialException();

            if (!user.IsActive)
                throw new UserNotActivatedException();

            var accessToken = await _tokenService.CreateAccessToken(user);
            var refreshToken = await _tokenService.CreateRefreshToken(user);

            user.LoginHistory ??= new List<LoginHistory>();
            
            user.LoginHistory.Add(new LoginHistory
            {
                Timestamp = DateTime.Now,
                UserAgent = httpContext.Request.Headers["User-Agent"],
                IP = httpContext.Connection.RemoteIpAddress?.ToString()
            });
            user.RefreshTokenSignature = refreshToken.Id;

            await _userRepository.UpdateAsync(user);

            return new AuthTokenDto
            {
                AccessToken = accessToken.ToBase64String(),
                RefreshToken = refreshToken.ToBase64String()
            };
        }

        public async Task DeleteUserAsync(long id)
        {
            if (!await _userRepository.ExistsAsync(user => user.Id == id))
            {
                throw new UserDoesNotExistException();
            }
            
            await _userRepository.DeleteAsync(id);
        }
    }
}