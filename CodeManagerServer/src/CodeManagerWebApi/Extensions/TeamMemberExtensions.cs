using CodeManager.Data.Entities;

namespace CodeManagerWebApi.Extensions
{
    public static class TeamMemberExtensions
    {
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