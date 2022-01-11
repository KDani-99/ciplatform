using System.Threading.Tasks;
using CodeManagerWebApi.DataTransfer;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Services
{
    public interface IUserService
    {
        public Task CreateUser(UserDto userDto);

        public Task<UserDto> GetUserAsync(long id);

        public Task<bool> ExistsAsync(long id);

        public Task<AuthTokenDto> LoginAsync(LoginDto userDto, HttpContext httpContext);
    }
}