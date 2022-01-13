using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Exceptions;
using CodeManagerWebApi.Repositories;

namespace CodeManagerWebApi.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        }

        public async Task CreateTeamAsync(TeamDto teamDto, User user)
        {
            if (!user.Roles.Contains(Roles.Admin))
            {
                if (user.Teams.Count >= user.Plan.MaxCreatedTeamsPerUser)
                {
                    throw new UserReachedMaxCreatedTeamsException();
                }
                            
                if (user.Teams.Count >= user.Plan.MaxJoinedTeamsPerUser)
                {
                    throw new UserReachedMaxJoinedTeamsException();
                }
            }

            if (await _teamRepository.ExistsAsync(teamEntity => teamEntity.Name == teamDto.Name))
            {
                throw new TeamAlreadyExistsException();
            }
            
            var team = new Team
            {
                Name = teamDto.Name,
                Image = teamDto.Image,
                Owner = user,
                Members = new List<TeamMember>
                {
                    new()
                    {
                        User = user,
                        Permission = Permissions.Admin
                    }
                }
            };

            await _teamRepository.CreateAsync(team);
        }

        public async Task DeleteTeamAsync(long id)
        {
            if (await _teamRepository.ExistsAsync(team => team.Id == id))
            {
                throw new TeamDoesNotExistException();
            }

            await _teamRepository.DeleteAsync(id);
        }
    }
}