using System.Collections.Generic;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Project;

namespace CIPlatformWebApi.Services
{
    public interface IProjectService
    {
        public Task<ProjectDataDto> GetProjectAsync(long id, User user);
        public Task<IEnumerable<ProjectDto>> GetProjectsAsync();
        public Task UpdateProjectAsync(long id, CreateProjectDto createProjectDto, User user);
        public Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, User user);
        public Task DeleteProjectAsync(long id, User user);
        public Task<bool> IsAllowedAsync(long id, User user);
    }
}