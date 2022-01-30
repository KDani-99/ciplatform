using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Exceptions;

namespace CodeManagerWebApi.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        }

        public async Task CreateTeamAsync(CreateTeamDto createTeamDto, User user)
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

            if (await _teamRepository.ExistsAsync(teamEntity => teamEntity.Name == createTeamDto.Name))
            {
                throw new TeamAlreadyExistsException();
            }
            
            var team = new Team
            {
                Name = createTeamDto.Name,
                Image = createTeamDto.Image,
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