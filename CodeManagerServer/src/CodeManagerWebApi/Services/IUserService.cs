using System.Threading.Tasks;
using CodeManagerWebApi.DataTransfer;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Services
{
    public interface IUserService
    {
        public Task CreateUser(CreateUserDto createUserDto);

        public Task<CreateUserDto> GetUserAsync(long id);

        public Task<bool> ExistsAsync(long id);

        public Task<AuthTokenDto> LoginAsync(LoginDto userDto, HttpContext httpContext);

        public Task DeleteUserAsync(long id);
    }
}