using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Commands;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Events;
using CIPlatform.Data.Extensions;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Configuration;
using CIPlatformManager.Exceptions;
using CIPlatformManager.WebSocket.Hubs;
using CIPlatformManager.WebSocket;
using IPlatformManager.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace CIPlatformManager.Services
{
    public class RunService : IRunService
    {
        private readonly AgentManagerConfiguration _agentManagerConfiguration;

        // Single unit of work, run and dispose
        private readonly IBusControl _busControl;
        private readonly IHubContext<WorkerHub, IWorkerClient> _hubContext;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IProjectRepository _projectRepository;
        private readonly IRunRepository _runRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IWorkerConnectionService _workerConnectionService;

        public RunService(IBusControl busControl,
                          JsonSerializerOptions jsonSerializerOptions,
                          IRunRepository runRepository,
                          IProjectRepository projectRepository,
                          IOptions<AgentManagerConfiguration> agentManagerConfiguration,
                          IHubContext<WorkerHub, IWorkerClient> hubContext,
                          IWorkerConnectionService workerConnectionService,
                          ITokenService<JwtSecurityToken> tokenService)
        {
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _jsonSerializerOptions =
                jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _agentManagerConfiguration = agentManagerConfiguration.Value ??
                throw new ArgumentNullException(nameof(agentManagerConfiguration));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _workerConnectionService = workerConnectionService ??
                throw new ArgumentNullException(nameof(workerConnectionService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<long> QueueAsync(QueueRunCommand cmd)
        {
            var project = await _projectRepository.GetAsync(cmd.ProjectId);

            InsertInitialStep(cmd.RunConfiguration, project);
            
            var run = await SaveRunConfigurationAsync(cmd.Repository, project, cmd.RunConfiguration);

            await SendRunQueuedNotification(project.Id, run.Id);

            foreach (var job in run.Jobs)
                await QueueJobAsync(run, job);
            
            await _runRepository.UpdateAsync(run);
            
            // TODO: send event to update status

            return run.Id;
        }

        private static void InsertInitialStep(RunConfiguration runConfiguration, Project project)
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
        
        private async Task QueueJobAsync(Run run, Job job)
        {
            var jobToken = await _tokenService.CreateJobTokenAsync(run.Id, job.Id);

            var workerConnectionId =
                (await _workerConnectionService.GetAvailableWorkerConnectionIdsOfTypeAsync(job.Context))
                .FirstOrDefault();
            // TODO: wait until a worker is available??
            if (workerConnectionId == null) throw new WorkersNotAvailableException();

            var jobConfiguration =
                JsonSerializer.Deserialize<JobConfiguration>(job.JsonContext, _jsonSerializerOptions);

            var queueJobEvent = new QueueJobCommand
            {
                Token = jobToken.ToBase64String(),
                JobConfiguration = jobConfiguration,
                Repository = run.Repository
            };

            switch (job.Context)
            {
                case JobContext.Docker:
                    await _hubContext.Clients.Client(workerConnectionId).QueueDockerJob(queueJobEvent);
                    break;
                case JobContext.Windows:
                    await _hubContext.Clients.Client(workerConnectionId).QueueWindowsJob(queueJobEvent);
                    break;
                case JobContext.Linux:
                    await _hubContext.Clients.Client(workerConnectionId).QueueLinuxJob(queueJobEvent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"The job context must be one of the following types: '{nameof(JobContext.Linux)}', '{nameof(JobContext.Windows)}', '{nameof(JobContext.Docker)}'");
            }

            await UpdateWorkerStateAsync(workerConnectionId);

            job.State = States.Running;
            job.StartedDateTime = DateTime.Now;
            
            
        }

        private async Task UpdateWorkerStateAsync(string connectionId)
        {
            var workerConnectionData = await _workerConnectionService.GetWorkerConnectionAsync(connectionId);
            workerConnectionData.AgentState = AgentState.Working;
            await _workerConnectionService.UpdateWorkerConnectionAsync(workerConnectionData);
        }

        private async Task<Run> SaveRunConfigurationAsync(string repository,
                                                          Project project,
                                                          RunConfiguration runConfiguration)
        {
            // TODO: let web API create the config
            var numberOfSteps = runConfiguration.Jobs.Aggregate(0, (result, current) => result + current.Value.Steps.Count);
            var run = new Run
            {
                Repository = repository,
                Jobs = runConfiguration.Jobs.Select((job, jobIndex) => new Job
                {
                    JsonContext = JsonSerializer.Serialize(job.Value, _jsonSerializerOptions),
                    Context = job.Value.Context,
                    Name = job.Key,
                    Index = jobIndex,
                    Steps = job.Value.Steps.Select((step, stepIndex) => new Step
                    {
                        Name = step.Name,
                        State = States.NotRun,
                        LogPath = null,
                        Index = stepIndex
                    }).ToList(),
                    State = States.NotRun
                }).ToList(),
                StartedDateTime = null,
                FinishedDateTime = null,
                State = States.Queued,
                NumberOfSteps = numberOfSteps,
                Project = project
            };

            await _runRepository.CreateAsync(run);

            return run;
        }
        
        private Task SendRunQueuedNotification(long projectId, long runId)
        {
            var processed = new RunQueuedEvent
            {
                ProjectId = projectId,
                RunId = runId
            };
            return _busControl.Publish(processed);
        }
    }
}