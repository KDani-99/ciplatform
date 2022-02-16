using CIPlatform.Data.Entities;

namespace CIPlatformWebApi.DataTransfer
{
    public class UpdateRoleDto
    {
        public long UserId { get; set; }
        public Permissions Role { get; set; }
    }
}