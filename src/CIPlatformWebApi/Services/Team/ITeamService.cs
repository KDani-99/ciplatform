using System.Collections.Generic;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Team;

namespace CIPlatformWebApi.Services.Team
{
    public interface ITeamService
    {
        public Task<IEnumerable<TeamDto>> GetTeamsAsync(UserEntity user);
        public Task<TeamDataDto> GetTeamAsync(long id, UserEntity user);
        public Task<TeamDto> CreateTeamAsync(TeamDto teamDto, UserEntity user);
        public Task UpdateTeamAsync(TeamDto teamDto, UserEntity user);
        public Task DeleteTeamAsync(long id, UserEntity user);
        public Task KickMemberAsync(long teamId, long memberId, UserEntity user);
        public Task AddMemberAsync(long teamId, AddMemberDto addMemberDto, UserEntity user);
        public Task UpdateRoleAsync(long teamId, UpdateRoleDto updateRoleDto, UserEntity user);
        public Task JoinAsync(long teamId, UserEntity user);
    }
}