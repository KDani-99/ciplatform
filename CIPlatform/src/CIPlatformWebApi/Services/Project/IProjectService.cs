using System.Collections.Generic;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Project;

namespace CIPlatformWebApi.Services.Project
{
    public interface IProjectService
    {
        public Task<ProjectDataDto> GetProjectAsync(long id, UserEntity user);
        public Task<IEnumerable<ProjectDto>> GetProjectsAsync();
        public Task UpdateProjectAsync(long id, CreateProjectDto createProjectDto, UserEntity user);
        public Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, UserEntity user);
        public Task DeleteProjectAsync(long id, UserEntity user);
        public Task<bool> IsAllowedAsync(long id, UserEntity user);
    }
}