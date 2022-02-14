using System.Linq;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Extensions.Entities
{
    public static class TeamExtensions
    {
        public static TeamDto ToDto(this Team team, long userId)
        {
            return new() // nyelvi elemeknek koszonhetoen igy adhatjuk meg
            {
                Name = team.Name,
                Description = team.Description,
                Id = team.Id,
                Owner = team.Owner.Username,
                IsMember = team.Members.Any(member => member.User.Id == userId),
                Image = team.Image,
                IsPublic = team.IsPublic,
                Members = team.Members?.Count ?? 0,
                Projects = team.Projects?.Count ?? 0
            };
        }

        public static TeamDataDto ToDataDto(this Team team, Permissions memberPermission)
        {
            return new ()
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                Image = team.Image,
                IsPublic = team.IsPublic,
                Owner = team.Owner.Username,
                Members = team.Members.Select(teamMember => teamMember.ToDto()),
                Projects = team.Projects.Select(project => project.ToDto()),
                UserPermission = memberPermission
            };
        }
    }
}