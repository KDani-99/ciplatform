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
        
        public async Task<Project> GetProjectAsync(long id, User user)
        {
            // check whether user is in the project
            var project = (await _projectRepository.GetAsync(id)) ?? throw new ProjectDoesNotExistException();

            if (!project.IsPrivateProject)
            {
                return project;
            }

            if (user.Teams.Any(team => team.Id == project.Team.Id))
            {
                throw new UnauthorizedAccessWebException("This project does not exist or you are not allowed to view this project.");
            }

            return project;
        }

        public async Task CreateProjectAsync(CreateProjectDto createProjectDto, User user)
        {
            if (user.Teams.All(team => team.Id != createProjectDto.TeamId))
            {
                throw new UnauthorizedAccessWebException("You are not a member of this team.");
            }

            await _projectRepository.CreateAsync(new Project
            {
                RepositoryUrl = createProjectDto.RepositoryUrl,
                SecretToken = createProjectDto.SecretToken, // TODO: encrypt
                Username = user.Username,
                Team = new Team { Id = createProjectDto.TeamId }, // wont create new team, just references the ID
                IsPrivateProject = createProjectDto.IsPrivateProject,
                IsPrivateRepository = createProjectDto.IsPrivateRepository,
            });
        }
    }
}