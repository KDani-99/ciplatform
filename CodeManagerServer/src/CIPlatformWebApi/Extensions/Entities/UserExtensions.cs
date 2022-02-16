using System.Linq;
using CIPlatform.Data.Entities;

namespace CIPlatformWebApi.Extensions.Entities
{
    public static class UserExtensions
    {
        public static bool IsAdmin(this User user)
        {
            return user.Roles.Any(role => role == Roles.Admin);
        }
    }
}