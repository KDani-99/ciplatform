using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Data.Agent.Requests;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities.CI;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Configuration;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgentManager.Services
{
    public class RunService : IRunService<QueueRunCommand>
    {
        private readonly AgentManagerConfiguration _agentManagerConfiguration;

        // Single unit of work, run and dispose
        private readonly IBusControl _busControl;
        private readonly ILogger<RunService> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<AgentRequestRequest> _requestClient; // OBSERVER PATTERN -> not needed?
        private readonly IRunRepository _runRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;

        public RunService(IRunRepository runRepository, ILogger<RunService> logger,
            IOptions<AgentManagerConfiguration> agentManagerConfiguration, IBusControl busControl,
            ITokenService<JwtSecurityToken> tokenService)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _agentManagerConfiguration = agentManagerConfiguration.Value ??
                                         throw new ArgumentNullException(nameof(agentManagerConfiguration));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<long> QueueAsync(QueueRunCommand cmd)
        {
            var run = await SaveRunConfiguration(cmd);

            var jobTasks = run.Jobs.Select(job => Task.Run(async () =>
            {
                var jobRequestToken =
                    await _tokenService.CreateJobRequestTokenAsync(run.Id,
                        job.Id); // this token will allow services to accept jobs

                var cancellationToken = CancellationToken.None; // TODO: add token
                await _publishEndpoint.Publish(new QueueJobEvent
                {
                    Token = jobRequestToken.ToBase64String()
                }, cancellationToken);
                // TODO: send job queued event back to the client

                job.State = States.Queued;
                await _runRepository.UpdateAsync(run); // TODO: use job repository?
            }));

            await Task.WhenAll(jobTasks);

            run.State = States.Queued;
            await _runRepository.UpdateAsync(run);


            return run.Id;
            // TODO: send run queued event?
        }


        private async Task<Run> SaveRunConfiguration(QueueRunCommand cmd)
        {
            var runConfiguration = new Run
            {
                Jobs = cmd.RunConfiguration.Jobs.Select(job => new Job
                {
                    Name = job.Key,
                    Steps = job.Value.Steps.Select(step => new Step
                    {
                        Name = step.Name,
                        State = States.NotRun,
                        LogPath = null
                    }).ToList(),
                    State = States.NotRun
                }).ToList(),
                ContextFilePath = cmd.ContextFilePath,
                State = States.NotRun
            };

            await _runRepository.CreateAsync(runConfiguration);

            return runConfiguration;
        }
    }
}