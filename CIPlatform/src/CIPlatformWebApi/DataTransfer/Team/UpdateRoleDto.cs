using CIPlatform.Data.Entities;

namespace CIPlatformWebApi.DataTransfer.User
{
    public class UpdateRoleDto
    {
        public long UserId { get; set; }
        public Permissions Role { get; set; }
    }
}