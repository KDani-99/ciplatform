using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CIPlatform.Data.Commands;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Job;
using CIPlatformWebApi.DataTransfer.Run;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Extensions.Entities;
using CIPlatformWebApi.Services.FileProcessor;
using MassTransit;

namespace CIPlatformWebApi.Services.Run
{
    public class RunService : IRunService
    {
        private readonly IBusControl _busControl;
        private readonly IFileProcessorService<RunConfiguration> _fileProcessorService;
        private readonly IProjectRepository _projectRepository;
        private readonly IRunRepository _runRepository;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public RunService(IRunRepository runRepository,
                          IProjectRepository projectRepository,
                          IBusControl busControl,
                          IFileProcessorService<RunConfiguration> fileProcessorService,
                          JsonSerializerOptions jsonSerializerOptions)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _fileProcessorService =
                fileProcessorService ?? throw new ArgumentNullException(nameof(fileProcessorService));
            _jsonSerializerOptions =
                jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
        }

        public async Task<RunDto> GetRunAsync(long runId)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            return run.ToDto();
        }

        public async Task<RunDto> GetRunAsync(long runId, UserEntity user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            VerifyMembership(run.Project, user);

            return run.ToDto();
        }

        public async Task<RunDataDto> GetRunDataAsync(long runId, UserEntity user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            VerifyMembership(run.Project, user);

            return run.ToDataDto();
        }

        public async Task<JobDataDto> GetJobAsync(long runId, long jobId, UserEntity user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            VerifyMembership(run.Project, user);

            var job = run.Jobs.FirstOrDefault(jobEntity => jobEntity.Id == jobId) ??
                throw new JobDoesNotExistException();

            return job.ToDataDto();
        }

        public async Task<Stream> GetStepFileStreamAsync(long runId, long jobId, long stepId, UserEntity user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            VerifyMembership(run.Project, user);

            var step = run.Jobs
                .FirstOrDefault(job => job.Id == jobId)
                ?.Steps.FirstOrDefault(s => s.Id == stepId) ?? throw new StepDoesNotExistException();

            // Opening it for Read only with ReadWrite files hare allows concurrent access
            return File.Open(step.LogPath, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite);
        }

        public async Task<RunDto> CreateRunAsync(long projectId, string instructions, UserEntity user)
        {
            var project = await _projectRepository.GetAsync(projectId) ?? throw new ProjectDoesNotExistException();

            var runConfiguration = await _fileProcessorService.ProcessAsync(instructions);

            VerifyMembership(project, user);

            InsertInitialSteps(runConfiguration, project);
            var runEntity = CreateRunEntity(project, runConfiguration);

            var runId = await _runRepository.CreateAsync(runEntity);
            // TODO: return dto

            await _busControl.Publish(new QueueRunCommand
            {
                RunId = runId
            });
            
            return runEntity.ToDto();
        }

        public async Task DeleteRunAsync(long runId, UserEntity user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            var member = run.Project.Team.Members.FirstOrDefault(teamMember => teamMember.User.Id == user.Id) ??
                throw new UnauthorizedAccessWebException("Only team members can delete runs.");
            
            if (!member.CanUpdateProjects() && !user.IsAdmin())
            {
                throw new UnauthorizedAccessWebException("You are not allowed to delete runs in this team.");
            }

            await _runRepository.DeleteAsync(runId);
        }

        public async Task<bool> IsAllowedRun(long runId, UserEntity user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();
            return run.Project.Team.Members.Any(member => member.Id == user.Id);
        }

        public async Task<bool> IsAllowedJob(long jobId, UserEntity user)
        {
            var result = (await _runRepository.GetAsync(run => run.Jobs.Any(job => job.Id == jobId))).FirstOrDefault() ?? throw new RunDoesNotExistException();
            return result.Project.Team.Members.Any(member => member.Id == user.Id);
        }

        public async Task<bool> IsAllowedStep(long stepId, UserEntity user)
        {
            var result = (await _runRepository.GetAsync(run => run.Jobs.Any(job => job.Steps.Any(step => step.Id == stepId)))).FirstOrDefault() ?? throw new RunDoesNotExistException();
            return result.Project.Team.Members.Any(member => member.Id == user.Id);
        }

        private static void VerifyMembership(ProjectEntity project, UserEntity user)
        {
            if (project.Team.Members.All(member => member.User.Id != user.Id))
            {
                throw new UnauthorizedAccessException("You are not a member of this team.");
            }
        }

        private RunEntity CreateRunEntity(ProjectEntity project , RunConfiguration runConfiguration)
        {
            var numberOfSteps = runConfiguration.Jobs.Aggregate(0, (result, current) => result + current.Value.Steps.Count);
            return new RunEntity
            {
                Repository = project.RepositoryUrl,
                Jobs = runConfiguration.Jobs.Select((job, jobIndex) => new JobEntity
                {
                    JsonContext = JsonSerializer.Serialize(job.Value, _jsonSerializerOptions),
                    Context = job.Value.Context,
                    Name = job.Key,
                    Index = jobIndex,
                    Steps = job.Value.Steps.Select((step, stepIndex) => new StepEntity
                    {
                        Name = step.Name,
                        State = States.NotRun,
                        LogPath = null,
                        Index = stepIndex
                    }).ToList(),
                    State = States.Queued
                }).ToList(),
                StartedDateTime = null,
                FinishedDateTime = null,
                State = States.NotRun,
                NumberOfSteps = numberOfSteps,
                Project = project
            };
        }
        private static void InsertInitialSteps(RunConfiguration runConfiguration, ProjectEntity project)
        {
            foreach (var job in runConfiguration.Jobs)
            {
                job.Value.Steps.Insert(0, new StepConfiguration
                {
                    Name = "Checkout repository (setup)",
                    Cmd = $"git clone {project.RepositoryUrl} [wd]" // wd = working directory
                });
            }
        }
    }

}