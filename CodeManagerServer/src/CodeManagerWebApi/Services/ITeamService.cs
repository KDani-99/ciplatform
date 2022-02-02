using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Services
{
    public interface ITeamService
    {
        public Task<IEnumerable<TeamDto>> GetTeamsAsync();
        public Task<TeamDto> GetTeamAsync(long id, User user);
        public Task<TeamDto> CreateTeamAsync(TeamDto teamDto, User user);
        public Task UpdateTeamAsync(TeamDto teamDto, User user);
        public Task DeleteTeamAsync(long id);
    }
}