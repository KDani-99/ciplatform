using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Utils.Extensions
{
    public static class UserExtensions
    {

        public static User FromDto(this CreateUserDto createUserDto) => new User
        {
            Username = createUserDto.Username,
            Password = createUserDto.Password
        };

        public static CreateUserDto FromUser(this User user) => new CreateUserDto
        {
            Username = user.Username,
            Password = user.Password,
        };
    }
}