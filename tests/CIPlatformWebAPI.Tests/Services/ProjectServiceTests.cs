using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformWebApi.DataTransfer.Project;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Services.Project;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.Services
{
    public class ProjectServiceTests
    {
        [Test]
        public void GetProjectAsync_InvalidId_ShouldThrowProjectDoesNotExistException()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0
            };
            var projectRepository = new Mock<IProjectRepository>();
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<ProjectDoesNotExistException>(() => projectService.GetProjectAsync(projectId, user));
        }
        
        [Test]
        public void GetProjectAsync_NotMember_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0
            };
            var projectEntity = new ProjectEntity
            {
                Team = new TeamEntity
                {
                    Members = new List<TeamMemberEntity>()
                }
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => projectService.GetProjectAsync(projectId, user));
        }
        
        [Test]
        public async Task GetProjectAsync_Member_ShouldReturnRunDto()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user
            };
            var projectEntity = new ProjectEntity
            {
                Team = new TeamEntity
                {
                    Members = new List<TeamMemberEntity> {teamMemberEntity}
                }
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act
            var result = await projectService.GetProjectAsync(projectId, user);
            
            // Assert
            Assert.NotNull(result);
            projectRepository.Verify(x => x.GetAsync(It.IsAny<long>()), Times.Once);
        }
        
        [Test]
        public async Task GetProjectsAsync_NoProjectsAvailable_ShouldThrowProjectDoesNotExistException()
        {
            // Arrange
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
                .Returns(Task.FromResult(new List<ProjectEntity>()));
            
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act
            var result = await projectService.GetProjectsAsync();
            
            // Act and Assert
            Assert.IsEmpty(result);
        }
        
        [Test]
        public void CreateProjectAsync_ExistingProject_ShouldThrowProjectAlreadyExistsException()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = 0
            };
            var createProjectDto = new CreateProjectDto();
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
                .Returns(Task.FromResult(true));
            
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<ProjectAlreadyExistsException>(() => projectService.CreateProjectAsync(createProjectDto, user));
        }
        
        [Test]
        public void CreateProjectAsync_InvalidTeam_ShouldThrowTeamDoesNotExistException()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = 0
            };
            var createProjectDto = new CreateProjectDto();
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
                .Returns(Task.FromResult(false));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity>()));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<TeamDoesNotExistException>(() => projectService.CreateProjectAsync(createProjectDto, user));
        }
        
        [Test]
        public void CreateProjectAsync_NotMember_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = 0
            };
            var createProjectDto = new CreateProjectDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>()
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
                .Returns(Task.FromResult(false));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => projectService.CreateProjectAsync(createProjectDto, user));
        }
        
        [Test]
        public void CreateProjectAsync_WithReadPermission_ShouldThrowTeamUnauthorizedAccessWebException()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user,
                Permission = Permissions.Read
            };
            var createProjectDto = new CreateProjectDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMemberEntity }
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
                .Returns(Task.FromResult(false));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => projectService.CreateProjectAsync(createProjectDto, user));
        }
        
        [Test]
        public void CreateProjectAsync_WithReadPermissionAndAdminRole_ShouldUpdate()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.Admin}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user,
                Permission = Permissions.Read
            };
            var createProjectDto = new CreateProjectDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMemberEntity }
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
                .Returns(Task.FromResult(false));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => projectService.CreateProjectAsync(createProjectDto, user));
            projectRepository.Verify(x => x.CreateAsync(It.IsAny<ProjectEntity>()), Times.Once);
        }
        
        [Test]
        public void CreateProjectAsync_WithReadAndWritePermission_ShouldUpdate()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user,
                Permission = Permissions.ReadWrite
            };
            var createProjectDto = new CreateProjectDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMemberEntity }
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
                .Returns(Task.FromResult(false));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => projectService.CreateProjectAsync(createProjectDto, user));
            projectRepository.Verify(x => x.CreateAsync(It.IsAny<ProjectEntity>()), Times.Once);
        }
        
        [Test]
        public void CreateProjectAsync_WithAdminPermission_ShouldUpdate()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user,
                Permission = Permissions.Admin
            };
            var createProjectDto = new CreateProjectDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMemberEntity }
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
                .Returns(Task.FromResult(false));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => projectService.CreateProjectAsync(createProjectDto, user));
            projectRepository.Verify(x => x.CreateAsync(It.IsAny<ProjectEntity>()), Times.Once);
        }
        
        [Test]
        public void UpdateProjectAsync_InvalidId_ShouldThrowProjectDoesNotExistException()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0
            };
            var projectDto = new CreateProjectDto();
            var projectRepository = new Mock<IProjectRepository>();
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<ProjectDoesNotExistException>(() => projectService.UpdateProjectAsync(projectId, projectDto, user));
        }
        
        [Test]
        public void UpdateProjectAsync_NotMember_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0
            };
            var projectEntity = new ProjectEntity
            {
                Team = new TeamEntity
                {
                    Members = new List<TeamMemberEntity>()
                }
            };
            var projectDto = new CreateProjectDto();
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => projectService.UpdateProjectAsync(projectId,projectDto, user));
        }
        
        [Test]
        public void UpdateProjectAsync_WithReadPermission_ShouldThrowUnauthorizedAccessWebException()
        {
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var createProjectDto = new CreateProjectDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>()
            };
            var projectEntity = new ProjectEntity
            {
                Team = teamEntity
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => projectService.UpdateProjectAsync(projectId, createProjectDto, user));
        }
        
        [Test]
        public void UpdateProjectAsync_WithReadPermissionAndAdminRole_ShouldUpdate()
        {
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.Admin}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user,
                Permission = Permissions.Read
            };
            var createProjectDto = new CreateProjectDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMemberEntity }
            };
            var projectEntity = new ProjectEntity
            {
                Team = teamEntity
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => projectService.UpdateProjectAsync(projectId, createProjectDto, user));
            projectRepository.Verify(x => x.UpdateAsync(It.IsAny<ProjectEntity>()), Times.Once);
        }
        
        [Test]
        public void UpdateProjectAsync_WithReadAndWritePermission_ShouldUpdate()
        {
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user,
                Permission = Permissions.ReadWrite
            };
            var createProjectDto = new CreateProjectDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMemberEntity }
            };
            var projectEntity = new ProjectEntity
            {
                Team = teamEntity
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => projectService.UpdateProjectAsync(projectId,createProjectDto, user));
            projectRepository.Verify(x => x.UpdateAsync(It.IsAny<ProjectEntity>()), Times.Once);
        }
        
        [Test]
        public void UpdateProjectAsync_WithAdminPermission_ShouldUpdate()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user,
                Permission = Permissions.Admin
            };
            var createProjectDto = new CreateProjectDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMemberEntity }
            };
            var projectEntity = new ProjectEntity
            {
                Team = teamEntity
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => projectService.UpdateProjectAsync(projectId, createProjectDto, user));
            projectRepository.Verify(x => x.UpdateAsync(It.IsAny<ProjectEntity>()), Times.Once);
        }
        
        [Test]
        public void DeleteProjectAsync_InvalidId_ShouldThrowProjectDoesNotExistException()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0
            };
            var projectRepository = new Mock<IProjectRepository>();
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<ProjectDoesNotExistException>(() => projectService.DeleteProjectAsync(projectId, user));
        }
        
        [Test]
        public void DeleteProjectAsync_NotMember_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0
            };
            var projectEntity = new ProjectEntity
            {
                Team = new TeamEntity
                {
                    Members = new List<TeamMemberEntity>()
                }
            };

            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => projectService.DeleteProjectAsync(projectId, user));
        }
        
        [Test]
        public void DeleteProjectAsync_WithReadPermission_ShouldThrowUnauthorizedAccessWebException()
        {
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>()
            };
            var projectEntity = new ProjectEntity
            {
                Team = teamEntity
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => projectService.DeleteProjectAsync(projectId, user));
        }
        
        [Test]
        public void DeleteProjectAsync_WithReadPermissionAndAdminRole_ShouldDelete()
        {
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.Admin}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user,
                Permission = Permissions.Read
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMemberEntity }
            };
            var projectEntity = new ProjectEntity
            {
                Team = teamEntity
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => projectService.DeleteProjectAsync(projectId, user));
            projectRepository.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.Once);
        }
        
        [Test]
        public void DeleteProjectAsync_WithReadAndWritePermission_ShouldDelete()
        {
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user,
                Permission = Permissions.ReadWrite
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMemberEntity }
            };
            var projectEntity = new ProjectEntity
            {
                Team = teamEntity
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => projectService.DeleteProjectAsync(projectId, user));
            projectRepository.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.Once);
        }
        
        [Test]
        public void DeleteProjectAsync_WithAdminPermission_ShouldUpdate()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user,
                Permission = Permissions.Admin
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMemberEntity }
            };
            var projectEntity = new ProjectEntity
            {
                Team = teamEntity
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity> {teamEntity}));
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => projectService.DeleteProjectAsync(projectId, user));
            projectRepository.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.Once);
        }
        
        [Test]
        public void IsAllowedAsync_InvalidId_ShouldThrowProjectDoesNotExistException()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0
            };
            var projectRepository = new Mock<IProjectRepository>();
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<ProjectDoesNotExistException>(() => projectService.IsAllowedAsync(projectId, user));
        }
        
        [Test]
        public async Task IsAllowedAsync_ValidId_ShouldReturnTrue()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = user
            };
            var projectEntity = new ProjectEntity
            {
                Team = new TeamEntity
                {
                    Members = new List<TeamMemberEntity> {teamMemberEntity}
                }
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);

            // Act
            var result = await projectService.IsAllowedAsync(projectId, user);
            
            // Assert
            Assert.True(result);
        }
        
        [Test]
        public async Task IsAllowedAsync_ValidId_ShouldReturnFalse()
        {
            // Arrange
            var projectId = 0;
            var user = new UserEntity
            {
                Id = 0
            };
            var projectEntity = new ProjectEntity
            {
                Team = new TeamEntity
                {
                    Members = new List<TeamMemberEntity>()
                }
            };
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(projectEntity));
            
            var teamRepository = new Mock<ITeamRepository>();
            
            var projectService = new ProjectService(projectRepository.Object, teamRepository.Object);

            // Act
            var result = await projectService.IsAllowedAsync(projectId, user);
            
            // Assert
            Assert.False(result);
        }
    }
}