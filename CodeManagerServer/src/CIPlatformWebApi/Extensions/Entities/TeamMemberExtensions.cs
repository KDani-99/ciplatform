using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;

namespace CIPlatformWebApi.Extensions.Entities
{
    public static class TeamMemberExtensions
    {
        public static TeamMemberDto ToDto(this TeamMember teamMember)
        {
            return new()
            {
                Id = teamMember.User.Id,
                Name = teamMember.User.Name,
                Username = teamMember.User.Username,
                JoinTime = teamMember.JoinTime,
                Permission = teamMember.Permission
            };
        }
        public static bool CanUpdateProjects(this TeamMember teamMember)
        {
            return teamMember.Permission is Permissions.ReadWrite or Permissions.Admin;
        }

        public static bool IsAdmin(this TeamMember teamMember)
        {
            return teamMember.Permission is Permissions.Admin;
        }
    }
}