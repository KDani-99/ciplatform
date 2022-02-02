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

        public async Task<IEnumerable<TeamDto>> GetTeamsAsync()
        {
            var teams = await _teamRepository.GetAsync((_ => true));

            return teams.Select(team => new TeamDto
            {
                Name = team.Name,
                Description = team.Description,
                Id = team.Id,
                Owner = team.Owner.Username,
                Image = team.Image,
                Members = team.Members?.Count ?? 0,
                Projects = team.Projects?.Count ?? 0
            });
        }

        public async Task<TeamDto> GetTeamAsync(long id, User user)
        {
            var team = await _teamRepository.GetAsync(id);

            if (team == null)
            {
                throw new TeamDoesNotExistException();
            }

            if (team.Members.All(teamMember => teamMember.Id != user.Id))
            {
                throw new UnauthorizedAccessWebException(
                    "The specified team does not exist or you are not allowed to view it.");
            }
            
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                Image = team.Image,
                IsPublic = team.IsPublic,
                Owner = team.Owner.Username,
                Members = team.Members?.Count ?? 0,
                Projects = team.Projects?.Count ?? 0
            };
        }

        public async Task<TeamDto> CreateTeamAsync(TeamDto teamDto, User user)
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
                Description = teamDto.Description,
                IsPublic = teamDto.IsPublic,
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

            var id = await _teamRepository.CreateAsync(team);

            teamDto.Id = id;
            teamDto.Owner = user.Username;

            return teamDto;
        }

        public async Task UpdateTeamAsync(TeamDto teamDto, User user)
        {
            var team = await _teamRepository.GetAsync(teamDto.Id);

            if (team == null)
            {
                throw new TeamDoesNotExistException();
            }

            if (team.Owner.Id != user.Id)
            {
                throw new UnauthorizedAccessWebException("Only the owner of the team can update the details.");
            }

            if (await _teamRepository.ExistsAsync(teamEntity => teamEntity.Name == teamDto.Name))
            {
                throw new TeamAlreadyExistsException();
            }

            team.Name = teamDto.Name;
            team.Description = teamDto.Description;
            team.Image = teamDto.Image;
            team.IsPublic = teamDto.IsPublic;

            await _teamRepository.UpdateAsync(team);
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