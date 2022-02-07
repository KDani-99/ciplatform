using CodeManager.Data.Entities;

namespace CodeManagerWebApi.DataTransfer
{
    public class UpdateRoleDto
    {
        public long UserId { get; set; }
        public Permissions Role { get; set; }
    }
}