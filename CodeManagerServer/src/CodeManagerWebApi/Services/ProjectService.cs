using System;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Exceptions;

namespace CodeManagerWebApi.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        }
        
        public async Task<ProjectDto> GetProjectAsync(long id, User user)
        {
            // check whether user is in the project
            var project = (await _projectRepository.GetAsync(id)) ?? throw new ProjectDoesNotExistException();

            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                IsPrivateProject = project.IsPrivateProject,
                IsPrivateRepository = project.IsPrivateProject,
                TeamId = project.Team.Id,
                RepositoryUrl = project.RepositoryUrl
            };

            if (!project.IsPrivateProject)
            {
                return projectDto;
            }

            if (user.Teams.All(team => team.Id != project.Team.Id))
            {
                throw new UnauthorizedAccessWebException("This project does not exist or you are not allowed to view this project.");
            }

            return projectDto;
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, User user)
        {
            if (await _projectRepository.ExistsAsync(projectEntity => projectEntity.Name == createProjectDto.Name && projectEntity.Team.Id == createProjectDto.TeamId))
            {
                throw new ProjectAlreadyExistsException();
            }
            
            var team = user.Teams.FirstOrDefault(t => t.Id == createProjectDto.TeamId);
            
            if (team == default)
            {
                throw new UnauthorizedAccessWebException("You are not a member of this team.");
            }

            var id = await _projectRepository.CreateAsync(new Project
            {
                Name = createProjectDto.Name,
                RepositoryUrl = createProjectDto.RepositoryUrl,
                SecretToken = createProjectDto.SecretToken, // TODO: encrypt
                Username = user.Username,
                Team = team, // wont create new team, just references the ID
                IsPrivateProject = createProjectDto.IsPrivateProject,
                IsPrivateRepository = createProjectDto.IsPrivateRepository,
            });

            return new ProjectDto
            {
                Id = id,
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                IsPrivateProject = createProjectDto.IsPrivateProject,
                IsPrivateRepository = createProjectDto.IsPrivateProject,
                TeamId = createProjectDto.TeamId,
                RepositoryUrl = createProjectDto.RepositoryUrl
            };
        }
    }
}