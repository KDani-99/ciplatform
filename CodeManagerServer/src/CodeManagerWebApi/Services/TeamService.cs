using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Exceptions;
using CodeManagerWebApi.Extensions;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IProjectRepository _projectRepository;

        public TeamService(ITeamRepository teamRepository, IProjectRepository projectRepository)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        }

        public async Task<IEnumerable<TeamDto>> GetTeamsAsync(User user)
        {
            var teams = await _teamRepository.GetAsync((_ => true));

            return teams.Select(team => new TeamDto
            {
                Name = team.Name,
                Description = team.Description,
                Id = team.Id,
                Owner = team.Owner.Username,
                IsMember = team.Members.Any(member => member.User.Id == user.Id),
                Image = team.Image,
                IsPublic = team.IsPublic,
                Members = team.Members?.Count ?? 0,
                Projects = team.Projects?.Count ?? 0
            });
        }

        public async Task<TeamDataDto> GetTeamAsync(long id, User user)
        {
            var team = await _teamRepository.GetAsync(id) ?? throw new TeamDoesNotExistException();;

            var member = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id);

            if (member == default)
            {
                throw new UnauthorizedAccessWebException(
                    "The specified team does not exist or you are not allowed to view it.");
            }
            
            return new TeamDataDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                Image = team.Image,
                IsPublic = team.IsPublic,
                Owner = team.Owner.Username,
                Members = team.Members.Select(teamMember  => new TeamMemberDto
                {
                    Id = teamMember.User.Id,
                    Name = teamMember.User.Name,
                    Username = teamMember.User.Username,
                    JoinTime = teamMember.JoinTime
                }),
                Projects = team.Projects.Select(project => new ProjectDto
                {
                    Id   = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    Owner = project.Team.Name,
                    TeamName = project.Team.Name, // TODO:
                    IsPrivateProject = project.IsPrivateProject,
                    Runs = project.Runs?.Count ?? 0
                }),
                UserPermission = member.Permission
            };
        }

        public async Task<TeamDto> CreateTeamAsync(TeamDto teamDto, User user)
        {
            if (!user.IsAdmin())
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
                        Permission = Permissions.Admin,
                        JoinTime = DateTime.Now
                    }
                }
            };

            var id = await _teamRepository.CreateAsync(team);

            teamDto.Id = id;
            teamDto.Owner = user.Username;
            teamDto.IsMember = true;
            teamDto.Members = 1;

            return teamDto;
        }

        public async Task UpdateTeamAsync(TeamDto teamDto, User user)
        {
            var team = await _teamRepository.GetAsync(teamDto.Id) ?? throw new TeamDoesNotExistException();

            var member = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id);

            if (!member.IsAdmin() && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException($"Only members with {nameof(Permissions.Admin)} can update the team.");
            }

            if (teamDto.Name != team.Name && await _teamRepository.ExistsAsync(teamEntity => teamEntity.Name == teamDto.Name))
            {
                throw new TeamAlreadyExistsException();
            }

            team.Name = teamDto.Name;
            team.Description = teamDto.Description;
            team.Image = teamDto.Image;
            team.IsPublic = teamDto.IsPublic;

            await _teamRepository.UpdateAsync(team);
        }

        public async Task DeleteTeamAsync(long id, User user)
        {
            var team = await _teamRepository.GetAsync(id) ?? throw new TeamDoesNotExistException();

            if (team.Owner.Id != user.Id && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("Only the owner can delete the team.");
            }

            /* TODO: remove team members

            foreach (var project in team.Projects)
            {
                await _projectRepository.DeleteAsync(project.Id);
            }*/

            await _teamRepository.DeleteAsync(id);
        }

        public async Task KickMemberAsync(long teamId, long memberId, User user)
        {
            var team = await _teamRepository.GetAsync(teamId) ?? throw new TeamDoesNotExistException();

            var member = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id);
            var memberToKick = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == memberId);

            if (memberToKick == default)
            {
                throw new BadHttpRequestException($"Member with id {memberId} is not in this team.", (int)HttpStatusCode.BadRequest);
            }

            if (team.Owner.Id == memberToKick.Id)
            {
                throw new UnauthorizedAccessWebException("Team owners can't be kicked.");
            }

            if (!member.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to kick members.");
            }

            team.Members = team.Members.Where(teamMember => teamMember.User.Id != memberId).ToList();

            await _teamRepository.UpdateAsync(team); 
        }

        public async Task JoinAsync(long teamId, User user)
        {
            var team = await _teamRepository.GetAsync(teamId) ?? throw new TeamDoesNotExistException();

            if (team.Members.Any(teamMember => teamMember.User.Id == user.Id))
            {
                throw new UserAlreadyInTeamException();
            }

            if (!user.IsAdmin() && !team.IsPublic)
            {
                throw new UnauthorizedAccessWebException("You are not allowed to join a private team.");
            }
            
            team.Members.Add(new TeamMember
            {
                User = user,
                JoinTime = DateTime.Now,
                Permission = Permissions.Read
            });
            
            await _teamRepository.UpdateAsync(team); 
        }
    }
}