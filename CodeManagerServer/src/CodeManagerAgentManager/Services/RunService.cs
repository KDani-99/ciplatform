using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Clients;
using CodeManager.Data.Agent;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Entities.CI;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Configuration;
using CodeManagerAgentManager.Exceptions;
using CodeManagerAgentManager.WebSocket;
using CodeManagerAgentManager.WebSocket.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgentManager.Services
{
    public class RunService : IRunService
    {
        private readonly AgentManagerConfiguration _agentManagerConfiguration;

        // Single unit of work, run and dispose
        private readonly IHubContext<AgentHub, IAgentClient> _hubContext;
        private readonly IManagerClient _managerClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IRunRepository _runRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IWorkerConnectionService _workerConnectionService;

        public RunService(JsonSerializerOptions jsonSerializerOptions,
            IRunRepository runRepository, IProjectRepository projectRepository,
            IOptions<AgentManagerConfiguration> agentManagerConfiguration,
            IHubContext<AgentHub, IAgentClient> hubContext, IManagerClient managerClient, IWorkerConnectionService workerConnectionService,
            ITokenService<JwtSecurityToken> tokenService)
        {
            _jsonSerializerOptions =
                jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _agentManagerConfiguration = agentManagerConfiguration.Value ??
                                         throw new ArgumentNullException(nameof(agentManagerConfiguration));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _managerClient = managerClient ?? throw new ArgumentNullException(nameof(managerClient));
            _workerConnectionService = workerConnectionService ??
                                       throw new ArgumentNullException(nameof(workerConnectionService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<long> QueueAsync(QueueRunCommand cmd)
        {
            var run = await SaveRunConfigurationAsync(cmd.Repository, cmd.ProjectId, cmd.RunConfiguration);
            run.State = States.Running;

            foreach (var job in run.Jobs)
                await QueueJobAsync(run, job);

            run.State = States.Queued;
            await _runRepository.UpdateAsync(run);

            return run.Id;
        }

        private async Task QueueJobAsync(Run run, Job job)
        {
            var jobToken = await _tokenService.CreateJobTokenAsync(run.Id, job.Id);

            var workerConnectionId =
                (await _workerConnectionService.GetAvailableWorkerConnectionIdsOfTypeAsync(job.Context))
                .FirstOrDefault();

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

            job.Steps.Insert(0, new Step
            {
                Name = "Checkout repository (setup)"
            });

            job.State = States.Running;
            job.StartDateTime = DateTime.Now;

            await NotifyWebApi(job);
        }

        private async Task UpdateWorkerStateAsync(string connectionId)
        {
            var workerConnectionData = await _workerConnectionService.GetWorkerConnectionAsync(connectionId);
            workerConnectionData.AgentState = AgentState.Working;
            await _workerConnectionService.UpdateWorkerConnectionAsync(workerConnectionData);
        }

        private async Task<Run> SaveRunConfigurationAsync(string repository, long projectId,
            RunConfiguration runConfiguration)
        {
            var project = await _projectRepository.GetAsync(projectId);
            
            var run = new Run
            {
                Repository = repository,
                Jobs = runConfiguration.Jobs.Select(job => new Job
                {
                    JsonContext = JsonSerializer.Serialize(job.Value, _jsonSerializerOptions),
                    Context = job.Value.Context,
                    Name = job.Key,
                    Steps = job.Value.Steps.Select(step => new Step
                    {
                        Name = step.Name,
                        State = States.NotRun,
                        LogPath = null
                    }).ToList(),
                    State = States.NotRun
                }).ToList(),
                StartedDateTime = DateTime.Now,
                State = States.NotRun,
                Project = project
            };

            await _runRepository.CreateAsync(run);

            return run;
        }

        private Task NotifyWebApi(Job job)
        {
            var processed = new ProcessedJobRequest
            {
                JobId = job.Id
            };
            return _managerClient.HubConnection.SendAsync(CommonWebApiMethods.JobQueueResponse, processed);
        }
    }
}