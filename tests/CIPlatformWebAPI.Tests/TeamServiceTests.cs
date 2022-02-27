using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformWebApi.DataTransfer.Team;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Services.Team;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests
{
    public class TeamServiceTests
    {
        [Test]
        public async Task GetTeamsAsync_InvalidId_ShouldReturnTeams()
        {
            // Arrange
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(new List<TeamEntity>()));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act
            var teams = await teamService.GetTeamsAsync(userEntity);
            
            // Assert
            teamRepository.Verify(x => x.GetAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()), Times.Once);
            Assert.NotNull(teams);
            Assert.IsEmpty(teams);
        }
        
        [Test]
        public void GetTeamAsync_InvalidId_ShouldThrowTeamDoesNotExistException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var teamRepository = new Mock<ITeamRepository>();
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<TeamDoesNotExistException>(() => teamService.GetTeamAsync(teamId, userEntity));
        }
        
        [Test]
        public void GetTeamAsync_UserNotAMember_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var team = new TeamEntity
            {
                Id = teamId,
                Members = new List<TeamMemberEntity>()
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(team));
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => teamService.GetTeamAsync(teamId, userEntity));
        }
        
        [Test]
        public async Task GetTeamAsync_ValidDetails_ShouldReturnTeamDto()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>
                {
                    teamMemberEntity
                },
                Projects = new List<ProjectEntity>(),
                Owner = userEntity
            };

            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Assert
            var result = await teamService.GetTeamAsync(teamId, userEntity);
            
            // Assert
            teamRepository.Verify(x => x.GetAsync(It.IsAny<long>()));
            Assert.NotNull(result);
        }
        
        [Test]
        public void CreateTeamAsync_ExistingName_ShouldThrowTeamAlreadyExistsException()
        {
            // Arrange
            var createTeamDto = new TeamDto
            {
                Name = "test",
            };
            var userEntity = new UserEntity();

            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(true));
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<TeamAlreadyExistsException>(() => teamService.CreateTeamAsync(createTeamDto, userEntity));
        }
        
        [Test]
        public async Task CreateTeamAsync_ValidDetails_ShouldReturnTeamDto()
        {
            // Arrange
            var createTeamDto = new TeamDto
            {
                Name = "test",
            };
            var userEntity = new UserEntity();

            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(false));
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act
            var result = await teamService.CreateTeamAsync(createTeamDto, userEntity);
            
            // Assert
            teamRepository.Verify(x => x.CreateAsync(It.IsAny<TeamEntity>()), Times.Once);
            Assert.NotNull(result);
        }
        
        [Test]
        public void UpdateTeamAsync_InvalidId_ShouldThrowTeamDoesNotExistException()
        {
            // Arrange
            var teamDto = new TeamDto();
            var userEntity = new UserEntity();
            var teamRepository = new Mock<ITeamRepository>();
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<TeamDoesNotExistException>(() => teamService.UpdateTeamAsync(teamDto, userEntity));
        }
        
        [Test]
        public void UpdateTeamAsync_NotAdmin_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var teamDto = new TeamDto();
            var userEntity = new UserEntity
            {
                Roles = new[] { Roles.User }
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.ReadWrite,
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>
                {
                    teamMemberEntity
                }
            };
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => teamService.UpdateTeamAsync(teamDto, userEntity));
        }
        
        [Test]
        public void UpdateTeamAsync_WithReadWritePermission_ShouldThrowTeamAlreadyExistsException()
        {
            // Arrange
            var teamDto = new TeamDto();
            var userEntity = new UserEntity
            {
                Roles = new[] { Roles.User }
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.Admin,
            };
            var teamEntity = new TeamEntity
            {
                Name = "test",
                Members = new List<TeamMemberEntity>
                {
                    teamMemberEntity
                }
            };
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            teamRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(true));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<TeamAlreadyExistsException>(() => teamService.UpdateTeamAsync(teamDto, userEntity));
        }
        
        [Test]
        public void UpdateTeamAsync_WithReadWritePermission_ShouldUpdate()
        {
            // Arrange
            var teamDto = new TeamDto();
            var userEntity = new UserEntity
            {
                Roles = new[] { Roles.User }
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.Admin,
            };
            var teamEntity = new TeamEntity
            {
                Name = "test",
                Members = new List<TeamMemberEntity>
                {
                    teamMemberEntity
                }
            };
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            teamRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(false));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.UpdateTeamAsync(teamDto, userEntity));
            teamRepository.Verify(x => x.UpdateAsync(It.IsAny<TeamEntity>()), Times.Once);
        }
        
        [Test]
        public void UpdateTeamAsync_WithReadWritePermissionAndAdminRole_ShouldUpdate()
        {
            // Arrange
            var teamDto = new TeamDto();
            var userEntity = new UserEntity
            {
                Roles = new[] { Roles.Admin }
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.ReadWrite,
            };
            var teamEntity = new TeamEntity
            {
                Name = "test",
                Members = new List<TeamMemberEntity>
                {
                    teamMemberEntity
                }
            };
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            teamRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>()))
                .Returns(Task.FromResult(false));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.UpdateTeamAsync(teamDto, userEntity));
            teamRepository.Verify(x => x.UpdateAsync(It.IsAny<TeamEntity>()), Times.Once);
        }
        
        [Test]
        public void DeleteTeamAsync_InvalidId_ShouldThrowTeamDoesNotExistException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var teamRepository = new Mock<ITeamRepository>();
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<TeamDoesNotExistException>(() => teamService.DeleteTeamAsync(teamId, userEntity));
        }
        
        [Test]
        public void DeleteTeamAsync_NotAdmin_ShouldThrowUnauthorizedAccessWebExceptinn()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity
            {
                Id = 0,
                Roles = new [] {Roles.User}
            };
            var otherUserEntity = new UserEntity
            {
                Id = 1,
                Roles = new [] {Roles.User}
            };
            var teamEntity = new TeamEntity
            {
                Id = teamId,
                Owner = otherUserEntity
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => teamService.DeleteTeamAsync(teamId, userEntity));
        }
        
        [Test]
        public void DeleteTeamAsync_NotOwnerWithAdminRole_ShouldDelete()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity
            {
                Id = 0,
                Roles = new [] {Roles.Admin}
            };
            var otherUserEntity = new UserEntity
            {
                Id = 1,
                Roles = new [] {Roles.User}
            };
            var teamEntity = new TeamEntity
            {
                Id = teamId,
                Owner = otherUserEntity
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.DeleteTeamAsync(teamId, userEntity));
            teamRepository.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.Once);
        }
        
        [Test]
        public void DeleteTeamAsync_Owner_ShouldDelete()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity
            {
                Id = 0,
                Roles = new [] {Roles.User}
            };
            var teamEntity = new TeamEntity
            {
                Id = teamId,
                Owner = userEntity
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.DeleteTeamAsync(teamId, userEntity));
            teamRepository.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.Once);
        }
        
        [Test]
        public void KickMemberAsync_InvalidId_ShoulThrowTeamDoesNotExistException()
        {
            // Arrange
            var teamId = 0;
            var memberId = 0;
            var userEntity = new UserEntity();
            var teamRepository = new Mock<ITeamRepository>();
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<TeamDoesNotExistException>(() => teamService.KickMemberAsync(teamId, memberId, userEntity));
        }
        
        [Test]
        public void KickMemberAsync_NotMember_ShouldThrowUserNotInTeamException()
        {
            // Arrange
            var teamId = 0;
            var memberId = 0;
            var userEntity = new UserEntity();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>()
            };
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UserNotInTeamException>(() => teamService.KickMemberAsync(teamId, memberId, userEntity));
        }
        
        [Test]
        public void KickMemberAsync_KickOwner_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var teamId = 0;
            var memberId = 0;
            var userId = 0;
            var userEntity = new UserEntity
            {
                Id = userId
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {teamMemberEntity},
                Owner = userEntity
            };
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => teamService.KickMemberAsync(teamId, memberId, userEntity));
        }
        
        [Test]
        public void KickMemberAsync_WithReadWritePermission_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var teamId = 0;
            var userId = 0;
            var otherUserId = 1;
            var userEntity = new UserEntity
            {
                Id = userId,
            };
            var otherUserEntity = new UserEntity
            {
                Id = otherUserId
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.Admin
            };
            var otherTeamMemberEntity = new TeamMemberEntity
            {
                User = otherUserEntity,
                Permission = Permissions.Read
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {teamMemberEntity, otherTeamMemberEntity},
                Owner = userEntity
            };
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => teamService.KickMemberAsync(teamId, otherUserId, otherUserEntity));
        }
        
        [Test]
        public void KickMemberAsync_ValidDetails_ShouldKick()
        {
            // Arrange
            var teamId = 0;
            var userId = 0;
            var otherUserId = 1;
            var userEntity = new UserEntity
            {
                Id = userId,
            };
            var otherUserEntity = new UserEntity
            {
                Id = otherUserId
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.Admin
            };
            var otherTeamMemberEntity = new TeamMemberEntity
            {
                User = otherUserEntity,
                Permission = Permissions.Read
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {teamMemberEntity, otherTeamMemberEntity},
                Owner = userEntity
            };
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);

            var expectedMemberSize = 1;
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.KickMemberAsync(teamId, otherUserId, userEntity));
            teamRepository.Verify(x => x.UpdateAsync(It.IsAny<TeamEntity>()), Times.Once);
            Assert.AreEqual(expectedMemberSize, teamEntity.Members.Count);
        }
        
        [Test]
        public void AddMemberAsync_InvalidUsername_ShouldThrowUserDoesNotExistException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var addMemberDto = new AddMemberDto();
            
            var teamRepository = new Mock<ITeamRepository>();
            
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(Task.FromResult(new List<UserEntity>()));
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);

            // Act and Assert
            Assert.ThrowsAsync<UserDoesNotExistException>(() => teamService.AddMemberAsync(teamId, addMemberDto, userEntity));
        }
        
        [Test]
        public void AddMemberAsync_InvalidId_ShouldThrowTeamDoesNotExistException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var addUserEntity = new UserEntity();
            var addMemberDto = new AddMemberDto();
            var teamRepository = new Mock<ITeamRepository>();
            
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(Task.FromResult(new List<UserEntity> {addUserEntity} ));
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<TeamDoesNotExistException>(() => teamService.AddMemberAsync(teamId, addMemberDto, userEntity));
        }
        
        [Test]
        public void AddMemberAsync_UserNotInTeam_ShouldThrowUserNotInTeamException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var addUserEntity = new UserEntity();
            var addMemberDto = new AddMemberDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>()
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(Task.FromResult(new List<UserEntity> {addUserEntity} ));
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UserNotInTeamException>(() => teamService.AddMemberAsync(teamId, addMemberDto, userEntity));
        }
        
        [Test]
        public void AddMemberAsync_UserAlreadyInTeam_ShouldThrowUserAlreadyInTeamException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var addUserEntity = new UserEntity();
            var memberToAddTeamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var addMemberDto = new AddMemberDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {memberToAddTeamMemberEntity}
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(Task.FromResult(new List<UserEntity> {addUserEntity} ));
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UserAlreadyInTeamException>(() => teamService.AddMemberAsync(teamId, addMemberDto, userEntity));
        }
        
        [Test]
        public void AddMemberAsync_WithReadWritePermission_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var teamId = 0;
            var userTryingToAddUserEntity = new UserEntity();
            var addUserEntity = new UserEntity
            {
                Id = 1,
            };
            var memberTryingToAddTeamMemberEntity = new TeamMemberEntity
            {
                User = userTryingToAddUserEntity,
                Permission = Permissions.ReadWrite
            };
            var addMemberDto = new AddMemberDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {memberTryingToAddTeamMemberEntity}
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(Task.FromResult(new List<UserEntity> {addUserEntity, userTryingToAddUserEntity} ));
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => teamService.AddMemberAsync(teamId, addMemberDto, userTryingToAddUserEntity));
        }
        
        [Test]
        public void AddMemberAsync_WithReadWritePermissionInPublicTeam_ShouldAdd()
        {
            // Arrange
            var teamId = 0;
            var userTryingToAddUserEntity = new UserEntity
            {
                Roles = new[] { Roles.Admin }
            };
            var addUserEntity = new UserEntity
            {
                Id = 1,
            };
            var memberTryingToAddTeamMemberEntity = new TeamMemberEntity
            {
                User = userTryingToAddUserEntity,
                Permission = Permissions.ReadWrite
            };
            var addMemberDto = new AddMemberDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {memberTryingToAddTeamMemberEntity},
                IsPublic = true
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(Task.FromResult(new List<UserEntity> {addUserEntity, userTryingToAddUserEntity} ));
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            var expectedTeamMembersSize = 2;
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.AddMemberAsync(teamId, addMemberDto, userTryingToAddUserEntity));
            teamRepository.Verify(x => x.UpdateAsync(It.IsAny<TeamEntity>()), Times.Once);
            Assert.AreEqual(expectedTeamMembersSize, teamEntity.Members.Count);
        }
        
        [Test]
        public void AddMemberAsync_WithReadPermissionInPublicTeam_ShouldAdd()
        {
            // Arrange
            var teamId = 0;
            var userTryingToAddUserEntity = new UserEntity
            {
                Roles = new[] { Roles.Admin }
            };
            var addUserEntity = new UserEntity
            {
                Id = 1,
            };
            var memberTryingToAddTeamMemberEntity = new TeamMemberEntity
            {
                User = userTryingToAddUserEntity,
                Permission = Permissions.Read
            };
            var addMemberDto = new AddMemberDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {memberTryingToAddTeamMemberEntity},
                IsPublic = true
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(Task.FromResult(new List<UserEntity> {addUserEntity, userTryingToAddUserEntity} ));
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            var expectedTeamMembersSize = 2;
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.AddMemberAsync(teamId, addMemberDto, userTryingToAddUserEntity));
            teamRepository.Verify(x => x.UpdateAsync(It.IsAny<TeamEntity>()), Times.Once);
            Assert.AreEqual(expectedTeamMembersSize, teamEntity.Members.Count);
        }
        
        [Test]
        public void AddMemberAsync_WithAdminPermissionInPrivateTeam_ShouldAdd()
        {
            // Arrange
            var teamId = 0;
            var userTryingToAddUserEntity = new UserEntity
            {
                Roles = new[] { Roles.Admin }
            };
            var addUserEntity = new UserEntity
            {
                Id = 1,
            };
            var memberTryingToAddTeamMemberEntity = new TeamMemberEntity
            {
                User = userTryingToAddUserEntity,
                Permission = Permissions.Admin
            };
            var addMemberDto = new AddMemberDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {memberTryingToAddTeamMemberEntity},
                IsPublic = true
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(Task.FromResult(new List<UserEntity> {addUserEntity, userTryingToAddUserEntity} ));
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            var expectedTeamMembersSize = 2;
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.AddMemberAsync(teamId, addMemberDto, userTryingToAddUserEntity));
            teamRepository.Verify(x => x.UpdateAsync(It.IsAny<TeamEntity>()), Times.Once);
            Assert.AreEqual(expectedTeamMembersSize, teamEntity.Members.Count);
        }
        
        [Test]
        public void UpdateRoleAsync_InvalidId_ShouldThrowTeamDoesNotExistException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var updateRoleDto = new UpdateRoleDto();
            var teamRepository = new Mock<ITeamRepository>();
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<TeamDoesNotExistException>(() => teamService.UpdateRoleAsync(teamId, updateRoleDto, userEntity));
        }
        
        [Test]
        public void UpdateRoleAsync_NotMember_ShouldThrowUserNotInTeamException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var updateRoleDto = new UpdateRoleDto();
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>()
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UserNotInTeamException>(() => teamService.UpdateRoleAsync(teamId, updateRoleDto, userEntity));
        }
        
        [Test]
        public void UpdateRoleAsync_UpdateOwnerRole_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var updateRoleDto = new UpdateRoleDto
            {
                UserId = 0
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {teamMemberEntity},
                Owner = userEntity
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => teamService.UpdateRoleAsync(teamId, updateRoleDto, userEntity));
        }
        
        [Test]
        public void UpdateRoleAsync_NotAdmin_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var userEntityToUpdate = new UserEntity
            {
                Id = 1
            };
            var updateRoleDto = new UpdateRoleDto
            {
                UserId = 1
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.ReadWrite
            };
            var teamMemberEntityToUpdate = new TeamMemberEntity
            {
                User = userEntityToUpdate
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {teamMemberEntity, teamMemberEntityToUpdate},
                Owner = userEntity
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => teamService.UpdateRoleAsync(teamId, updateRoleDto, userEntity));
        }
        
        [Test]
        public void UpdateRoleAsync_Admin_ShouldUpdate()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var userEntityToUpdate = new UserEntity
            {
                Id = 1
            };
            var updateRoleDto = new UpdateRoleDto
            {
                UserId = 1,
                Role = Permissions.ReadWrite
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.Admin
            };
            var teamMemberEntityToUpdate = new TeamMemberEntity
            {
                User = userEntityToUpdate
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> {teamMemberEntity, teamMemberEntityToUpdate},
                Owner = userEntity
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            var expectedPermission = Permissions.ReadWrite;
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.UpdateRoleAsync(teamId, updateRoleDto, userEntity));
            teamRepository.Verify(x => x.UpdateAsync(It.IsAny<TeamEntity>()), Times.Once());
            Assert.AreEqual(expectedPermission, teamMemberEntityToUpdate.Permission);
        }
        
        [Test]
        public void JoinTeamAsync_InvalidId_ShouldThrowTeamDoesNotExistException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var teamRepository = new Mock<ITeamRepository>();
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<TeamDoesNotExistException>(() => teamService.JoinAsync(teamId, userEntity));
        }
        
        [Test]
        public void JoinTeamAsync_AlreadyInTeam_ShouldThrowUserAlreadyInTeamException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity();
            var teamMember = new TeamMemberEntity
            {
                User = userEntity
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity> { teamMember }
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UserAlreadyInTeamException>(() => teamService.JoinAsync(teamId, userEntity));
        }
        
        [Test]
        public void JoinTeamAsync_PrivateTeam_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity
            {
                Roles = new[] { Roles.User }
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>(),
                IsPublic = false
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => teamService.JoinAsync(teamId, userEntity));
        }
        
        [Test]
        public void JoinTeamAsync_PrivateTeamWithAdminRole_ShouldJoin()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity
            {
                Roles = new[] { Roles.Admin }
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>(),
                IsPublic = false
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            var expectedSize = 1;
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.JoinAsync(teamId, userEntity));
            teamRepository.Verify(x => x.UpdateAsync(It.IsAny<TeamEntity>()), Times.Once);
            Assert.AreEqual(expectedSize, teamEntity.Members.Count);
        }
        
        [Test]
        public void JoinTeamAsync_PublicTeamWithoutAdminRole_ShouldJoin()
        {
            // Arrange
            var teamId = 0;
            var userEntity = new UserEntity
            {
                Roles = new[] { Roles.User }
            };
            var teamEntity = new TeamEntity
            {
                Members = new List<TeamMemberEntity>(),
                IsPublic = true
            };
            
            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(teamEntity));
            
            var userRepository = new Mock<IUserRepository>();
            
            var teamService = new TeamService(teamRepository.Object, userRepository.Object);
            var expectedSize = 1;
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => teamService.JoinAsync(teamId, userEntity));
            teamRepository.Verify(x => x.UpdateAsync(It.IsAny<TeamEntity>()), Times.Once);
            Assert.AreEqual(expectedSize, teamEntity.Members.Count);
        }
    }
}