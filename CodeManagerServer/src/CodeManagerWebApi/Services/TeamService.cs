using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Repositories;
using MongoDB.Driver;

namespace CodeManagerWebApi.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        }

        public async Task CreateTeam(TeamDto teamDto, User user)
        {
            // find teams where owner id == user.id

            if (!user.Roles.Contains(Roles.Admin))
            {
                if (user.Teams.Count >= user.Plan.MaxCreatedTeamsPerUser)
                {
                    // throw
                    return;
                }
                            
                if (user.Teams.Count >= user.Plan.MaxJoinedTeamsPerUser)
                {
                    // throw
                }
            }

            
            var team = new Team
            {
                Name = teamDto.Name,
                Image = teamDto.Image,
                Owner = user,
                Members = new List<TeamMember>
                {
                    new TeamMember
                    {
                        User = user,
                        Permission = Permissions.Admin
                    }
                }
            };

            await _teamRepository.CreateAsync(team);
        }
    }
}