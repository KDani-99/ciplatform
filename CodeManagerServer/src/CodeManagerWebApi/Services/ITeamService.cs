using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Services
{
    public interface ITeamService
    {
        public Task CreateTeamAsync(TeamDto teamDto, User user);
        public Task DeleteTeamAsync(long id);
    }
}