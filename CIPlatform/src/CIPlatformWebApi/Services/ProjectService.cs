using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Extensions;
using CIPlatformWebApi.Extensions.Entities;

namespace CIPlatformWebApi.Services
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
            var project = await _projectRepository.GetAsync(id) ?? throw new ProjectDoesNotExistException();

            var member = project.Team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ??
                throw new UnauthorizedAccessWebException(
                    "The specified project does not exist or you are not allowed to view it.");

            var projectDto = project.ToDataDto(member.Permission);

            if (!project.IsPrivateProject)
            {
                return projectDto;
            }

            if (user.Teams.All(team => team.Id != project.Team.Id))
            {
                throw new UnauthorizedAccessWebException(
                    "This project does not exist or you are not allowed to view this project.");
            }

            return projectDto;
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsAsync()
        {
            var projects = await _projectRepository.GetAsync(_ => true);

            return projects.Select(project => project.ToDto());
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, User user)
        {
            if (await _projectRepository.ExistsAsync(projectEntity =>
                                                         projectEntity.Name == createProjectDto.Name &&
                                                         projectEntity.Team.Id == createProjectDto.TeamId))
            {
                throw new ProjectAlreadyExistsException();
            }

            var team = (await _teamRepository.GetAsync(t => t.Id == createProjectDto.TeamId)).FirstOrDefault() ??
                throw new TeamDoesNotExistException();
            var member = team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ??
                throw new UnauthorizedAccessWebException("You are not a member of this team.");

            if (!member.CanUpdateProjects() && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to create projects.");
            }

            var id = await _projectRepository.CreateAsync(new Project
            {
                Name = createProjectDto.Name,
                RepositoryUrl = createProjectDto.RepositoryUrl,
                Username = user.Username,
                Description = createProjectDto.Description,
                Team = team,
                IsPrivateProject = createProjectDto.IsPrivateProject
            });

            return new ProjectDto
            {
                Id = id,
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                IsPrivateProject = createProjectDto.IsPrivateProject,
                TeamName = team.Name,
                Owner = team.Name,
                Runs = 0
            };
        }

        public async Task UpdateProjectAsync(long id, CreateProjectDto createProjectDto, User user)
        {
            var project = await _projectRepository.GetAsync(id) ?? throw new ProjectDoesNotExistException();

            var member = project.Team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ??
                throw new UnauthorizedAccessWebException("Only team members can update the project.");

            if (!member.CanUpdateProjects() && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to update the project.");
            }

            if (createProjectDto.Name != project.Name && await _projectRepository.ExistsAsync(
                projectEntity => projectEntity.Name == createProjectDto.Name &&
                    projectEntity.Team.Id == createProjectDto.TeamId))
            {
                throw new ProjectAlreadyExistsException();
            }

            project.Name = createProjectDto.Name;
            project.Description = createProjectDto.Description;
            project.IsPrivateProject = createProjectDto.IsPrivateProject;

            await _projectRepository.UpdateAsync(project);
        }

        public async Task DeleteProjectAsync(long id, User user)
        {
            var project = await _projectRepository.GetAsync(id) ?? throw new TeamDoesNotExistException();

            var member = project.Team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ??
                throw new UnauthorizedAccessWebException("Only team members can delete the project.");
            if (!member.CanUpdateProjects() && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to delete the project.");
            }

            await _projectRepository.DeleteAsync(id);
        }

        public async Task<bool> IsAllowedAsync(long id, User user)
        {
            var project = await _projectRepository.GetAsync(id) ?? throw new ProjectDoesNotExistException();
            return project.Team.Members.Any(teamMember => teamMember.User.Id == user.Id);
        }
    }
}