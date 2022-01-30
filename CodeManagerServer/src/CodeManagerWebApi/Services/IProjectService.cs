using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Services
{
    public interface IProjectService
    {
        public Task<Project> GetProjectAsync(long id, User user);
        public Task CreateProjectAsync(CreateProjectDto createProjectDto, User user);
    }
}