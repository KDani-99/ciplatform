using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CIPlatform.Data.Commands;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Services.FileProcessor;
using CIPlatformWebApi.Services.Run;
using MassTransit;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests
{
    public class RunServiceTests
    {
        [Test]
        public void GetRunDataAsync_InvalidId_ShouldThrowRunDoesNotExistException()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var runRepository = new Mock<IRunRepository>();
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<RunDoesNotExistException>(() => runService.GetRunDataAsync(runId, userEntity));
        }
        
        [Test]
        public void GetRunDataAsync_NotMember_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>()
                    }
                }
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => runService.GetRunDataAsync(runId, userEntity));
        }
        
        [Test]
        public async Task GetRunDataAsync_Member_ShouldReturnRunDto()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMemberEntity
                        }
                    }
                },
                Jobs = new List<JobEntity>()
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object, projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act
            var result = await runService.GetRunDataAsync(runId, userEntity);
            
            // Assert
            Assert.NotNull(result);
        }
        
        [Test]
        public void GetJobAsync_InvalidId_ShouldThrowRunDoesNotExistException()
        {
            // Arrange
            var jobId = 0;
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var runRepository = new Mock<IRunRepository>();
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<RunDoesNotExistException>(() => runService.GetJobAsync(runId, jobId, userEntity));
        }
        
        [Test]
        public void GetJobAsync_NotMember_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var jobId = 0;
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>()
                    }
                }
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => runService.GetJobAsync(runId,  jobId, userEntity));
        }
        
        [Test]
        public void GetJobAsync_InvalidJob_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var jobId = 0;
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMemberEntity
                        }
                    }
                },
                Jobs = new List<JobEntity>()
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<JobDoesNotExistException>(() => runService.GetJobAsync(runId,  jobId, userEntity));
        }
        
        [Test]
        public async Task GetJobAsync_Member_ShouldReturnJobDto()
        {
            // Arrange
            var runId = 0;
            var jobId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMemberEntity
                        }
                    }
                },
                Jobs = new List<JobEntity>
                {
                    new()
                    {
                        Steps = new List<StepEntity>(),
                        Context = JobContext.Docker
                    }
                }
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object, projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act
            var result = await runService.GetJobAsync(runId, jobId, userEntity);
            
            // Assert
            Assert.NotNull(result);
        }
        
        [Test]
        public void GetStepFileStreamAsync_InvalidId_ShouldThrowRunDoesNotExistException()
        {
            // Arrange
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var runRepository = new Mock<IRunRepository>();
            var projectRepository = new Mock<IProjectRepository>();
            var fileSystem = new Mock<IFileSystem>();
            var busControl = new Mock<IBusControl>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<RunDoesNotExistException>(() => runService.GetStepFileStreamAsync(runId, jobId, stepId, userEntity));
        }
        
        [Test]
        public void GetStepFileStreamAsync_NotMember_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>()
                    }
                }
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => runService.GetStepFileStreamAsync(runId, jobId, stepId, userEntity));
        }
        
        [Test]
        public async Task GetStepFileStreamAsync_NotMember_ShouldReturnFileStream()
        {
            // Arrange
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var teamMember = new TeamMemberEntity
            {
                User = userEntity
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMember
                        }
                    }
                },
                Jobs = new List<JobEntity>
                {
                    new()
                    {
                        Steps = new List<StepEntity>
                        {
                            new()
                            {
                                LogPath = "test"
                            }
                        }
                    }
                }
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var stream = new MemoryStream();
            
            var file = new Mock<IFile>();
            file.Setup(x => x.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
                .Returns(stream);
            
            var fileSystem = new Mock<IFileSystem>();
            fileSystem.SetupGet(x => x.File)
                .Returns(file.Object);
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object, projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act
            var result = await runService.GetStepFileStreamAsync(runId, jobId, stepId, userEntity);
            
            // Assert
            Assert.NotNull(result);
        }
        
        [Test]
        public void CreateRunAsync_InvalidId_ShouldThrowProjectDoesNotExistException()
        {
            // Arrange
            var projectId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var instructions = string.Empty;
            var runRepository = new Mock<IRunRepository>();
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<ProjectDoesNotExistException>(() => runService.CreateRunAsync(projectId, instructions, userEntity));
        }
        
        [Test]
        public void CreateRunAsync_NotMember_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var projectId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var project = new ProjectEntity
            {
                Team = new TeamEntity
                {
                    Members = new List<TeamMemberEntity>()
                }
            };
            var runRepository = new Mock<IRunRepository>();

            var instructions = string.Empty;
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(project));
            
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => runService.CreateRunAsync(projectId, instructions, userEntity));
        }
        
        [Test]
        public async Task CreateRunAsync_Member_ShouldCreate()
        {
            // Arrange
            var projectId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var project = new ProjectEntity
            {
                Team = new TeamEntity
                {
                    Members = new List<TeamMemberEntity>
                    {
                        teamMemberEntity
                    }
                }
            };
            var runConfiguration = new RunConfiguration
            {
                Jobs = new Dictionary<string, JobConfiguration>()
            };
            var runRepository = new Mock<IRunRepository>();

            var instructions = string.Empty;
            
            var projectRepository = new Mock<IProjectRepository>();
            projectRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(project));
            
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            fileProcessorService.Setup(x => x.ProcessAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(runConfiguration));
            
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act
            var result = await runService.CreateRunAsync(projectId, instructions, userEntity);
            
            // Assert
            busControl.Verify(x => x.Publish(It.IsAny<QueueRunCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(result);
        }
        
        [Test]
        public void DeleteRunAsync_InvalidId_ShouldThrowRunDoesNotExistException()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var runRepository = new Mock<IRunRepository>();
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<RunDoesNotExistException>(() => runService.DeleteRunAsync(runId, userEntity));
        }
        
        [Test]
        public void DeleteRunAsync_NotMember_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>()
                    }
                }
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => runService.DeleteRunAsync(runId, userEntity));
        }
        
        [Test]
        public void DeleteRunAsync_WithReadPermission_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.Read
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMemberEntity
                        }
                    }
                }
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() => runService.DeleteRunAsync(runId, userEntity));
        }
        
        [Test]
        public void DeleteRunAsync_WithReadWritePermission_ShouldUpdate()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.ReadWrite
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMemberEntity
                        }
                    }
                }
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => runService.DeleteRunAsync(runId, userEntity));
            runRepository.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.Once);
        }
        
        [Test]
        public void DeleteRunAsync_WithAdminPermission_ShouldUpdate()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.User}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.Admin
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMemberEntity
                        }
                    }
                }
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => runService.DeleteRunAsync(runId, userEntity));
            runRepository.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.Once);
        }
        
        [Test]
        public void DeleteRunAsync_WithReadPermissionAndAdminRole_ShouldUpdate()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0,
                Roles = new[] {Roles.Admin}
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity,
                Permission = Permissions.Read
            };
            var run = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMemberEntity
                        }
                    }
                }
            };
            var runRepository = new Mock<IRunRepository>();

            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object, jsonSerializerOptions);
            
            // Act and Assert
            Assert.DoesNotThrowAsync(() => runService.DeleteRunAsync(runId, userEntity));
            runRepository.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.Once);
        }
        
        [Test]
        public void IsAllowedRun_InvalidId_ShouldThrowRunDoesNotExistException()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var runRepository = new Mock<IRunRepository>();
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<RunDoesNotExistException>(() => runService.IsAllowedRun(runId, userEntity));
        }
        
        [Test]
        public async Task IsAllowedRun_ValidId_ShouldReturnTrue()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var runEntity = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMemberEntity
                        }
                    }
                }
            };
            
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(runEntity));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act
            var result = await runService.IsAllowedRun(runId, userEntity);
            
            // Act and Assert
            Assert.True(result);
        }
        
        [Test]
        public async Task IsAllowedRun_ValidId_ShouldReturnFalse()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var runEntity = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>()
                    }
                }
            };
            
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(runEntity));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act
            var result = await runService.IsAllowedRun(runId, userEntity);
            
            // Act and Assert
            Assert.False(result);
        }
        
        [Test]
        public void IsAllowedJob_InvalidId_ShouldThrowRunDoesNotExistException()
        {
            // Arrange
            var jobId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RunEntity, bool>>>()))
                .Returns(Task.FromResult(new List<RunEntity>()));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<RunDoesNotExistException>(() => runService.IsAllowedJob(jobId, userEntity));
        }
        
        [Test]
        public async Task IsAllowedJob_ValidId_ShouldReturnTrue()
        {
            // Arrange
            var jobId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var runEntity = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMemberEntity
                        }
                    }
                }
            };
            
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RunEntity, bool>>>()))
                .Returns(Task.FromResult(new List<RunEntity> {runEntity}));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act
            var result = await runService.IsAllowedJob(jobId, userEntity);
            
            // Act and Assert
            Assert.True(result);
        }
        
        [Test]
        public async Task IsAllowedJob_ValidId_ShouldReturnFalse()
        {
            // Arrange
            var jobId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var runEntity = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>()
                    },
                    Runs = new List<RunEntity>()
                }
            };
            
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RunEntity, bool>>>()))
                .Returns(Task.FromResult(new List<RunEntity> {runEntity}));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act
            var result = await runService.IsAllowedJob(jobId, userEntity);
            
            // Act and Assert
            Assert.False(result);
        }
        
         [Test]
        public void IsAllowedStep_InvalidId_ShouldThrowRunDoesNotExistException()
        {
            // Arrange
            var stepId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RunEntity, bool>>>()))
                .Returns(Task.FromResult(new List<RunEntity>()));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act and Assert
            Assert.ThrowsAsync<RunDoesNotExistException>(() => runService.IsAllowedStep(stepId, userEntity));
        }
        
        [Test]
        public async Task IsAllowedStep_ValidId_ShouldReturnTrue()
        {
            // Arrange
            var jobId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var teamMemberEntity = new TeamMemberEntity
            {
                User = userEntity
            };
            var runEntity = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>
                        {
                            teamMemberEntity
                        }
                    }
                }
            };
            
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RunEntity, bool>>>()))
                .Returns(Task.FromResult(new List<RunEntity> {runEntity}));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act
            var result = await runService.IsAllowedStep(jobId, userEntity);
            
            // Act and Assert
            Assert.True(result);
        }
        
        [Test]
        public async Task IsAllowedStep_ValidId_ShouldReturnFalse()
        {
            // Arrange
            var jobId = 0;
            var userEntity = new UserEntity
            {
                Id = 0
            };
            var runEntity = new RunEntity
            {
                Project = new ProjectEntity
                {
                    Team = new TeamEntity
                    {
                        Members = new List<TeamMemberEntity>()
                    },
                    Runs = new List<RunEntity>()
                }
            };
            
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RunEntity, bool>>>()))
                .Returns(Task.FromResult(new List<RunEntity> {runEntity}));
            
            var projectRepository = new Mock<IProjectRepository>();
            var busControl = new Mock<IBusControl>();
            var fileSystem = new Mock<IFileSystem>();
            var fileProcessorService = new Mock<IFileProcessorService<RunConfiguration>>();
            var jsonSerializerOptions = new JsonSerializerOptions();
            
            var runService = new RunService(runRepository.Object,projectRepository.Object, busControl.Object, fileProcessorService.Object, fileSystem.Object,jsonSerializerOptions);
            
            // Act
            var result = await runService.IsAllowedJob(jobId, userEntity);
            
            // Act and Assert
            Assert.False(result);
        }
    }
}