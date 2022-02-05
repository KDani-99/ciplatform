using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Services
{
    public interface IUserService
    {
        public Task CreateUser(CreateUserDto createUserDto);

        public Task<UserDto> GetUserAsync(long id, User user);
        public Task<IEnumerable<UserDto>> GetUsersAsync(User user);

        public Task<bool> ExistsAsync(long id);

        public Task<AuthTokenDto> LoginAsync(LoginDto userDto, HttpContext httpContext);

        public Task DeleteUserAsync(long id);
    }
}