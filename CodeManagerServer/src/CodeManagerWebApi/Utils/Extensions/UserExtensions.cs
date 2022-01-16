using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Utils.Extensions
{
    public static class UserExtensions
    {

        public static User FromDto(this UserDto userDto) => new User
        {
            Username = userDto.Username,
            Password = userDto.Password
        };

        public static UserDto FromUser(this User user) => new UserDto
        {
            Username = user.Username,
            Password = user.Password,
        };
    }
}