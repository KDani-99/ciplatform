using System;
using System.IO;
using System.Linq;
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
using MassTransit;

namespace CIPlatformWebApi.Services
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

            return run.ToDto();
        }

        public async Task<RunDataDto> GetRunAsync(long runId, User user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            VerifyMembership(run.Project, user);

            return run.ToDataDto();
        }

        public async Task<JobDataDto> GetJobAsync(long runId, long jobId, User user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            VerifyMembership(run.Project, user);

            var job = run.Jobs.FirstOrDefault(jobEntity => jobEntity.Id == jobId) ??
                throw new JobDoesNotExistException();

            return job.ToDataDto();
        }

        public async Task<Stream> GetStepFileStreamAsync(long runId, long jobId, long stepId, User user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            VerifyMembership(run.Project, user);

            var step = run.Jobs
                .FirstOrDefault(job => job.Id == jobId)
                ?.Steps.FirstOrDefault(s => s.Id == stepId) ?? throw new StepDoesNotExistException();

            return File.Open(step.LogPath, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite); // third param allows concurrent access to the file
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

        public async Task DeleteRunAsync(long runId, User user)
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

        public async Task<bool> IsAllowedRun(long runId, User user)
        {
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();
            return run.Project.Team.Members.Any(member => member.Id == user.Id);
        }

        public async Task<bool> IsAllowedJob(long jobId, User user)
        {
            var result = (await _runRepository.GetAsync(run => run.Jobs.Any(job => job.Id == jobId))).FirstOrDefault() ?? throw new RunDoesNotExistException();
            return result.Project.Team.Members.Any(member => member.Id == user.Id);
        }

        public async Task<bool> IsAllowedStep(long stepId, User user)
        {
            var result = (await _runRepository.GetAsync(run => run.Jobs.Any(job => job.Steps.Any(step => step.Id == stepId)))).FirstOrDefault() ?? throw new RunDoesNotExistException();
            return result.Project.Team.Members.Any(member => member.Id == user.Id);
        }

        private static void VerifyMembership(Project project, User user)
        {
            if (project.Team.Members.All(member => member.User.Id != user.Id))
            {
                throw new UnauthorizedAccessException("You are not a member of this team.");
            }
        }
    }
}