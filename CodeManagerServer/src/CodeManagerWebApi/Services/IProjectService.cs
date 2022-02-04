using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Services
{
    public interface IProjectService
    {
        public Task<ProjectDto> GetProjectAsync(long id, User user);
        public Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, User user);
    }
}