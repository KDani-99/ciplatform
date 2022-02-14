using System.Linq;
using CodeManager.Data.Entities;

namespace CodeManagerWebApi.Extensions
{
    public static class UserExtensions
    {
        public static bool IsAdmin(this User user)
        {
            return user.Roles.Any(role => role == Roles.Admin);
        }
    }
}