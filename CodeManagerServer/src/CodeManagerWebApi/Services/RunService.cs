using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Exceptions;
using MassTransit;

namespace CodeManagerWebApi.Services
{
    public class RunService : IRunService
    {
        private readonly IBusControl _busControl;
        private readonly IFileProcessorService<RunConfiguration> _fileProcessorService;
        private readonly IProjectRepository _projectRepository;
        private readonly IRunRepository _runRepository;

        public RunService(IRunRepository runRepository,
                          IProjectRepository projectRepository,
                          IBusControl busControl,
                          IFileProcessorService<RunConfiguration> fileProcessorService)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _fileProcessorService =
                fileProcessorService ?? throw new ArgumentNullException(nameof(fileProcessorService));
        }

        public async Task<RunDto> GetRunAsync(long runId)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            return new RunDto
            {
                Id = run.Id,
                StartedDateTime = run.StartedDateTime,
                FinishedDateTime = run.FinishedDateTime,
                State = run.State,
                Jobs = run.Jobs.Count,
            };
        }

        public async Task<RunDataDto> GetRunAsync(long runId, User user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            VerifyMembership(run.Project, user);

            return new RunDataDto
            {
                Run = new RunDto
                {
                    Id = run.Id,
                    StartedDateTime = run.StartedDateTime,
                    FinishedDateTime = run.FinishedDateTime,
                    State = run.State,
                    Jobs = run.Jobs.Count
                },
                Jobs = run.Jobs.Select(job => new JobDto
                {
                    Id = job.Id,
                    State = job.State,
                    StartedDateTime = job.StartedDateTime,
                    FinishedDateTime = job.FinishedDateTime,
                    Name = job.Name,
                    JobContext = job.Context.ToString(),
                    Steps = job.Steps.Count
                })
            };
        }

        public async Task<JobDataDto> GetJobAsync(long runId, long jobId, User user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            VerifyMembership(run.Project, user);

            var job = run.Jobs.FirstOrDefault(jobEntity => jobEntity.Id == jobId) ??
                throw new JobDoesNotExistException();

            return new JobDataDto
            {
                Job = new JobDto
                {
                    Id = job.Id,
                    State = job.State,
                    StartedDateTime = job.StartedDateTime,
                    FinishedDateTime = job.FinishedDateTime,
                    Name = job.Name,
                    JobContext = job.Context.ToString(),
                    Steps = job.Steps.Count
                },
                Steps = job.Steps.Select(step => new StepDto
                {
                    Id = step.Id,
                    Name = step.Name,
                    StartedDateTime = step.StartedDateTime,
                    FinishedDateTime = step.FinishedDateTime,
                    State = step.State
                })
            };
        }

        public async Task<Stream> GetStepFileStreamAsync(long runId, long jobId, long stepId, User user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            VerifyMembership(run.Project, user);

            var step = run.Jobs
                          .FirstOrDefault(job => job.Id == jobId)
                          ?.Steps.FirstOrDefault(s => s.Id == stepId) ?? throw new StepDoesNotExistException();

            /*var file = await File.ReadAllLinesAsync(step.LogPath);

            return new StepFileDto
            {
                StepLogs = file.Select((content, index) => new StepLogDto
                {
                    Line = index,
                    Content = content
                })
            };*/
            
            return File.Open(step.LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); // third param allows concurrent access to the file
        }

        public async Task QueueRunAsync(long projectId, string instructions, User user)
        {
            var project = await _projectRepository.GetAsync(projectId) ?? throw new ProjectDoesNotExistException();

            var runConfiguration = await _fileProcessorService.ProcessAsync(instructions, project.Id);

            VerifyMembership(project, user);

            await _busControl.Publish(new QueueRunCommand
            {
                RunConfiguration = runConfiguration,
                Repository = project.RepositoryUrl,
                ProjectId = project.Id
            });
        }

        private static void VerifyMembership(Project project, User user)
        {
            if (project.Team.Members.All(member => member.User.Id != user.Id))
                throw new UnauthorizedAccessException("You are not a member of this team.");
        }
    }
}