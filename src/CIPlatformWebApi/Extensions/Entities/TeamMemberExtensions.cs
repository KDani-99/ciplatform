using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Team;

namespace CIPlatformWebApi.Extensions.Entities
{
    public static class TeamMemberExtensions
    {
        public static TeamMemberDto ToDto(this TeamMemberEntity teamMember)
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

        public static bool CanUpdateProjects(this TeamMemberEntity teamMember)
        {
            return teamMember.Permission is Permissions.ReadWrite or Permissions.Admin;
        }

        public static bool IsAdmin(this TeamMemberEntity teamMember)
        {
            return teamMember.Permission is Permissions.Admin;
        }
    }
}