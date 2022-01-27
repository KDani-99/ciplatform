using System;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        }
        
        public Task<Project> GetProjectAsync(long id)
        {
            return _projectRepository.GetAsync(id);
        }

        public Task CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            // TODO: verify details
            return _projectRepository.CreateAsync(new Project
            {

            });
        }
    }
}