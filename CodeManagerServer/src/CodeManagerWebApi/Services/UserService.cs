using System;
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

        public async Task CreateUser(CreateUserDto createUserDto)
        {
            if (await _userRepository.GetByUsernameAsync(createUserDto.Username) is not null)
                throw new UsernameTakenException();

            if (await _userRepository.GetByEmailAsync(createUserDto.Email) is not null)
                throw new EmailAlreadyInUseException();

            var defaultPlan = (await _planRepository.GetAsync(plan => plan.Name == _userConfiguration.DefaultPlan)).FirstOrDefault();

            var user = new User
            {
                Username = createUserDto.Username,
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Image = null,
                Password = _credentialManagerService.CreateHashedPassword(createUserDto.Password),
                Roles = new [] {Roles.User},
                RegistrationTimestamp = DateTime.Now,
                Plan = defaultPlan
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
                Image = selectedUser.Image,
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
                Image = dbUser.Image,
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

            if (user is null) throw new UserDoesNotExistException();

            if (!_credentialManagerService.VerifyPassword(loginDto.Password, user.Password))
                throw new InvalidCredentialException();

            var accessToken = await _tokenService.CreateAccessToken(user);
            var refreshToken = await _tokenService.CreateRefreshToken(user);
            
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