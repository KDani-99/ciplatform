using CIPlatform.Data.Entities;

namespace CIPlatformWebApi.DataTransfer.Team
{
    public class UpdateRoleDto
    {
        public long UserId { get; set; }
        public Permissions Role { get; set; }
    }
}