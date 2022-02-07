using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Exceptions;
using CodeManagerWebApi.Extensions;

namespace CodeManagerWebApi.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITeamRepository _teamRepository;

        public ProjectService(IProjectRepository projectRepository, ITeamRepository teamRepository)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        }
        
        public async Task<ProjectDataDto> GetProjectAsync(long id, User user)
        {
            // check whether user is in the project
            var project = (await _projectRepository.GetAsync(id)) ?? throw new ProjectDoesNotExistException();

            var member = project.Team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ?? throw new UnauthorizedAccessWebException("The specified project does not exist or you are not allowed to view it.");;

            var projectDto = new ProjectDataDto
            {
                Project = new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    IsPrivateProject = project.IsPrivateProject,
                },
                TeamId = project.Team.Id,
                IsPrivateRepository = project.IsPrivateRepository,
                UserPermission = member.Permission,
                RepositoryUrl = project.RepositoryUrl,
                Variables = project.Variables.Select(variable => new VariableDto
                {
                    Name = variable.Name,
                    Value = variable.IsSecret ? "******" : variable.Value,
                    IsSecret = variable.IsSecret
                })
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

        public async Task<IEnumerable<ProjectDto>> GetProjectsAsync()
        {
            var projects = await _projectRepository.GetAsync((_) => true);
            
            return projects.Select(project => new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                IsPrivateProject = project.IsPrivateProject,
                TeamName = project.Team.Name,
                Owner = project.Team.Name,
            });
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, User user)
        {
            if (await _projectRepository.ExistsAsync(projectEntity => projectEntity.Name == createProjectDto.Name && projectEntity.Team.Id == createProjectDto.TeamId))
            {
                throw new ProjectAlreadyExistsException();
            }

            var team = (await _teamRepository.GetAsync(t => t.Id == createProjectDto.TeamId)).FirstOrDefault() ??
                       throw new TeamDoesNotExistException();
            var member = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id ) ?? throw new UnauthorizedAccessWebException("You are not a member of this team.");;;

            if (!member.CanUpdateProjects() && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to create projects.");
            }

            var id = await _projectRepository.CreateAsync(new Project
            {
                Name = createProjectDto.Name,
                RepositoryUrl = createProjectDto.RepositoryUrl,
                SecretToken = createProjectDto.SecretToken, // TODO: encrypt
                Username = user.Username,
                Description = createProjectDto.Description,
                Team = team,
                IsPrivateProject = createProjectDto.IsPrivateProject,
                IsPrivateRepository = createProjectDto.IsPrivateRepository,
            });

            return new ProjectDto
            {
                Id = id,
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                IsPrivateProject = createProjectDto.IsPrivateProject,
                TeamName = team.Name,
                Owner = team.Name,
                Runs = 0,
            };
        }
        
        public async Task UpdateProjectAsync(long id, CreateProjectDto createProjectDto, User user)
        {
            var project = await _projectRepository.GetAsync(id) ?? throw new ProjectDoesNotExistException();

            var member = project.Team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ?? throw new UnauthorizedAccessWebException("Only team members can update the project.");
            
            if (!member.CanUpdateProjects() && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to update the project.");
            }

            if (createProjectDto.Name != project.Name && await _projectRepository.ExistsAsync(projectEntity => projectEntity.Name == createProjectDto.Name && projectEntity.Team.Id == createProjectDto.TeamId))
            {
                throw new ProjectAlreadyExistsException();
            }

            project.Name = createProjectDto.Name;
            project.Description = createProjectDto.Description;
            project.IsPrivateProject = createProjectDto.IsPrivateProject;
            project.IsPrivateRepository = createProjectDto.IsPrivateRepository;
            project.Username = createProjectDto.Username;
            project.SecretToken = createProjectDto.SecretToken;

            await _projectRepository.UpdateAsync(project);
        }
        
        public async Task DeleteProjectAsync(long id, User user)
        {
            var project = await _projectRepository.GetAsync(id) ??  throw new TeamDoesNotExistException();;

            var member = project.Team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ?? throw new UnauthorizedAccessWebException("Only team members can delete the project.");;

            if (!member.CanUpdateProjects() && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to delete the project.");
            }

            // TODO: remove runs/jobs/steps

            await _projectRepository.DeleteAsync(id);
        }
    }
}