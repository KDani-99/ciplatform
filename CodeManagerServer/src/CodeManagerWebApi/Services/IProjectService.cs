using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Services
{
    public interface IProjectService
    {
        public Task<ProjectDto> GetProjectAsync(long id, User user);
        public Task<IEnumerable<ProjectDto>> GetProjectsAsync();
        public Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, User user);
    }
}