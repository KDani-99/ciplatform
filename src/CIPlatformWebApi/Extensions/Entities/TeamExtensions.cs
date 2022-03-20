using System.Linq;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Team;

namespace CIPlatformWebApi.Extensions.Entities
{
    public static class TeamExtensions
    {
        public static TeamDto ToDto(this TeamEntity team, long userId)
        {
            return new()
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

        public static TeamDataDto ToDataDto(this TeamEntity team, Permissions memberPermission)
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