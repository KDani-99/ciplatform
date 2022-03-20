using System.Collections.Generic;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.User;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Services.User
{
    public interface IUserService
    {
        public Task CreateUserAsync(CreateUserDto createUserDto);

        public Task<UserDto> GetUserAsync(long id, UserEntity user);
        public Task<IEnumerable<UserDto>> GetUsersAsync(UserEntity user);

        public Task<AuthTokenDto> LoginAsync(LoginDto userDto, HttpContext httpContext);
        public Task<AuthTokenDto> GenerateAuthTokensAsync(string username);
        public Task UpdateUserAsync(long id, UpdateUserDto updateUserDto, UserEntity user);

        public Task DeleteUserAsync(long id, UserEntity user);
    }
}