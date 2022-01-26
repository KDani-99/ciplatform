using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Entities.CI;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Configuration;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CodeManagerAgentManager.Services
{
    public class RunService : IRunService<QueueRunCommand>
    {
        private readonly AgentManagerConfiguration _agentManagerConfiguration;

        // Single unit of work, run and dispose
        private readonly IBusControl _busControl;
        private readonly ILogger<RunService> _logger;
        private readonly IRunRepository _runRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IFileProcessorService<RunConfiguration> _runConfigurationFileProcessorService;

        public RunService(JsonSerializerOptions jsonSerializerOptions, IRunRepository runRepository, ILogger<RunService> logger,
            IOptions<AgentManagerConfiguration> agentManagerConfiguration, IBusControl busControl,
            ITokenService<JwtSecurityToken> tokenService, IFileProcessorService<RunConfiguration> runConfigurationFileProcessorService)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _agentManagerConfiguration = agentManagerConfiguration.Value ??
                                         throw new ArgumentNullException(nameof(agentManagerConfiguration));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _runConfigurationFileProcessorService = runConfigurationFileProcessorService ?? throw new ArgumentNullException(nameof(runConfigurationFileProcessorService));
        }

        public async Task<long> QueueAsync(QueueRunCommand cmd)
        {
            var runConfiguration =
                await _runConfigurationFileProcessorService.ProcessAsync(cmd.RunConfigurationString, cmd.ProjectId);
            
            var run = await SaveRunConfiguration(cmd.Repository, cmd.ProjectId, runConfiguration);

            foreach (var job in run.Jobs)
            {
                var jobRequestToken =
                    await _tokenService.CreateJobRequestTokenAsync(run.Id,
                        job.Id); // this token will allow services to accept jobs

                var cancellationToken = CancellationToken.None; // TODO: add token

                var base64Token = jobRequestToken.ToBase64String();

                object queueJobEvent = job.Context switch
                {
                    JobContext.Docker => new QueueDockerJobEvent {Token = base64Token},
                    JobContext.Linux => new QueueLinuxJobEvent {Token = base64Token},
                    JobContext.Windows => new QueueWindowsJobEvent {Token = base64Token},
                    _ => throw new ArgumentOutOfRangeException($"The job context must be one of the following types: '{nameof(JobContext.Linux)}', '{nameof(JobContext.Windows)}', '{nameof(JobContext.Docker)}'")
                };

                await _busControl.Publish(queueJobEvent);
                // TODO: send job queued event back to the client

                job.Steps.Insert(0, new Step
                {
                    Name = "Checkout repository (setup)"
                });
                
                job.State = States.Queued;
                await _runRepository.UpdateAsync(run); // TODO: use job repository?
            }

            run.State = States.Queued;
            await _runRepository.UpdateAsync(run);


            return run.Id;
            // TODO: send run queued event?
        }


        private async Task<Run> SaveRunConfiguration(string repository, long projectId, RunConfiguration runConfiguration)
        {
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
                State = States.NotRun,
                Project = new Project {Id = projectId}
            };

            await _runRepository.CreateAsync(run);

            return run;
        }
    }
}