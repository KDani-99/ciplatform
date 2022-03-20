using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Team;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Extensions;
using CIPlatformWebApi.Extensions.Entities;

namespace CIPlatformWebApi.Services.Team
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;

        public TeamService(ITeamRepository teamRepository,
                           IUserRepository userRepository)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<IEnumerable<TeamDto>> GetTeamsAsync(UserEntity user)
        {
            var teams = await _teamRepository.GetAsync(_ => true);

            return teams.Select(team => team.ToDto(user.Id));
        }

        public async Task<TeamDataDto> GetTeamAsync(long id, UserEntity user)
        {
            var team = await _teamRepository.GetAsync(id) ?? throw new TeamDoesNotExistException();

            var member = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id);

            if (member == default)
            {
                throw new UnauthorizedAccessWebException(
                    "The specified team does not exist or you are not allowed to view it.");
            }

            return team.ToDataDto(member.Permission);
        }

        public async Task<TeamDto> CreateTeamAsync(TeamDto teamDto, UserEntity user)
        {
            if (await _teamRepository.ExistsAsync(teamEntity => teamEntity.Name == teamDto.Name))
            {
                throw new TeamAlreadyExistsException();
            }

            var team = new TeamEntity
            {
                Name = teamDto.Name,
                Image = teamDto.Image,
                Description = teamDto.Description,
                IsPublic = teamDto.IsPublic,
                Owner = user,
                Members = new List<TeamMemberEntity>
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

        public async Task UpdateTeamAsync(TeamDto teamDto, UserEntity user)
        {
            var team = await _teamRepository.GetAsync(teamDto.Id) ?? throw new TeamDoesNotExistException();

            var member = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id);

            if (!member.IsAdmin() && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException(
                    $"Only members with {nameof(Permissions.Admin)} can update the team.");
            }

            if (teamDto.Name != team.Name &&
                await _teamRepository.ExistsAsync(teamEntity => teamEntity.Name == teamDto.Name))
            {
                throw new TeamAlreadyExistsException();
            }

            team.Name = teamDto.Name;
            team.Description = teamDto.Description;
            team.Image = teamDto.Image;
            team.IsPublic = teamDto.IsPublic;

            await _teamRepository.UpdateAsync(team);
        }

        public async Task DeleteTeamAsync(long id, UserEntity user)
        {
            var team = await _teamRepository.GetAsync(id) ?? throw new TeamDoesNotExistException();

            if (team.Owner.Id != user.Id && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("Only the owner can delete the team.");
            }

            await _teamRepository.DeleteAsync(id);
        }

        public async Task KickMemberAsync(long teamId, long memberId, UserEntity user)
        {
            var team = await _teamRepository.GetAsync(teamId) ?? throw new TeamDoesNotExistException();

            var member = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ??
                throw new UserNotInTeamException();
            var memberToKick = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == memberId) ??
                throw new UserNotInTeamException($"Member with id {memberId} is not in this team.");

            if (team.Owner.Id == memberToKick.User.Id)
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

        public async Task AddMemberAsync(long teamId, AddMemberDto addMemberDto, UserEntity user)
        {
            var userToAdd =
                (await _userRepository.GetAsync(repoUser => repoUser.Username == addMemberDto.Username))
                .FirstOrDefault() ?? throw new UserDoesNotExistException();

            var team = await _teamRepository.GetAsync(teamId) ?? throw new TeamDoesNotExistException();

            var member = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ??
                throw new UserNotInTeamException("You must be in the team to kick members.");

            if (team.Members.Any(teamMember => teamMember.User.Id == userToAdd.Id))
            {
                throw new UserAlreadyInTeamException();
            }

            if (!member.IsAdmin() && !team.IsPublic)
            {
                throw new UnauthorizedAccessWebException("You are not allowed to add members.");
            }

            team.Members.Add(new TeamMemberEntity
            {
                Permission = Permissions.Read,
                JoinTime = DateTime.Now,
                User = userToAdd
            });

            await _teamRepository.UpdateAsync(team);
        }

        public async Task UpdateRoleAsync(long teamId, UpdateRoleDto updateRoleDto, UserEntity user)
        {
            var team = await _teamRepository.GetAsync(teamId) ?? throw new TeamDoesNotExistException();

            var member = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ??
                throw new UserNotInTeamException("You must be in the team to update the role of members.");
            var memberToUpdate =
                team.Members.FirstOrDefault(teamMember => teamMember.User.Id == updateRoleDto.UserId) ??
                throw new UserNotInTeamException($"Member with id {updateRoleDto.UserId} is not in this team.");

            if (team.Owner.Id == memberToUpdate.User.Id)
            {
                throw new UnauthorizedAccessWebException("Team owner's role can't be changes.");
            }

            if (!member.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to update roles.");
            }

            memberToUpdate.Permission = updateRoleDto.Role;

            await _teamRepository.UpdateAsync(team);
        }

        public async Task JoinAsync(long teamId, UserEntity user)
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

            team.Members.Add(new TeamMemberEntity
            {
                User = user,
                JoinTime = DateTime.Now,
                Permission = Permissions.Read
            });

            await _teamRepository.UpdateAsync(team);
        }
    }
}