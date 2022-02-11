using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Utils.Extensions
{
    public static class UserExtensions
    {
        public static User FromDto(this CreateUserDto createUserDto)
        {
            return new()
            {
                Username = createUserDto.Username,
                Password = createUserDto.Password
            };
        }

        public static CreateUserDto FromUser(this User user)
        {
            return new()
            {
                Username = user.Username,
                Password = user.Password
            };
        }
    }
}