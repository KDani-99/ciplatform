using System.Threading.Tasks;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Services
{
    public interface ITeamService
    {
        public Task CreateTeam(TeamDto teamDto, User user);
    }
}