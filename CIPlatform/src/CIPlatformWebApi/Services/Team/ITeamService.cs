﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Team;

namespace CIPlatformWebApi.Services
{
    public interface ITeamService
    {
        public Task<IEnumerable<TeamDto>> GetTeamsAsync(User user);
        public Task<TeamDataDto> GetTeamAsync(long id, User user);
        public Task<TeamDto> CreateTeamAsync(TeamDto teamDto, User user);
        public Task UpdateTeamAsync(TeamDto teamDto, User user);
        public Task DeleteTeamAsync(long id, User user);
        public Task KickMemberAsync(long teamId, long memberId, User user);
        public Task AddMemberAsync(long teamId, AddMemberDto addMemberDto, User user);
        public Task UpdateRoleAsync(long teamId, UpdateRoleDto updateRoleDto, User user);
        public Task JoinAsync(long teamId, User user);
    }
}