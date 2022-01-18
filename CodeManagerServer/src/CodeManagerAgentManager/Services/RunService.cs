using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Data.Agent;
using CodeManager.Data.Agent.Requests;
using CodeManager.Data.Agent.Responses;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Entities.CI;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.Messaging;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Commands;
using CodeManagerAgentManager.Configuration;
using Docker.DotNet;
using Docker.DotNet.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgentManager.Services
{
    public class RunService : IRunService
    {
        // Single unit of work, run and dispose
        private readonly IBusControl _busControl;
        private readonly IRunRepository _runRepository;
        private readonly IRequestClient<AgentRequestRequest> _requestClient; // OBSERVER PATTERN -> not needed?
        private readonly ILogger<RunService> _logger;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly AgentManagerConfiguration _agentManagerConfiguration;

        public RunService(IRunRepository runRepository, ILogger<RunService> logger, IOptions<AgentManagerConfiguration> agentManagerConfiguration, IBusControl busControl, ITokenService<JwtSecurityToken> tokenService)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _agentManagerConfiguration = agentManagerConfiguration.Value ??
                                         throw new ArgumentNullException(nameof(agentManagerConfiguration));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task StartAsync(StartRunCommand cmd)
        {
            try
            {
                var run = await SaveRunConfiguration(cmd);

                var jobTasks = run.Jobs.Select((job) => Task.Run(async () =>
                {
                    var jobRequestToken = await _tokenService.CreateJobRequestTokenAsync(run.Id,job.Id); // this token will allow services to accept jobs

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

                // TODO: send run queued event?
            }
            catch (RequestTimeoutException exception)
            {
                // TODO:
            }
            catch (Exception exception)
            {

            }
        }
        
        public async Task StartAsync(StartRunCommand cmd)
        {
            try
            {
                // Parallel job execution
                var run = await SaveRunConfiguration(cmd);

                // TODO: add config processor microservice? -> to handle config.yml parsing (custom config)

                var jobTasks = run.Jobs.Select((job) => Task.Run(async () =>
                {
                    // Request an agent for the job
                    
                    var response = await _requestClient
                        .Create(new AgentRequestRequest(), // TODO: remove agent request
                            timeout: TimeSpan.FromSeconds(_agentManagerConfiguration.AgentRequestTimeoutInSeconds))
                        .GetResponse<AgentRequestResponse>();
                    
                    // Send start job signal with job config
                    
                    var jobToken = await _tokenService.CreateJobTokenAsync(run.Id,job.Id);
                    
                    // TODO: below code might not work
                    var sendEndpoint = await _busControl.GetSendEndpoint(response.ResponseAddress);
   
                    await sendEndpoint.Send(new StartAgentJob
                    {
                        Token = jobToken.ToBase64String()
                    });
      
                }));

                await Task.WhenAll(jobTasks);
                
                // wait for agent response, send broadcast

            }
            catch (RequestTimeoutException exception)
            {
                // TODO:
            }
            catch (Exception exception)
            {

            }
        }
        

        private async Task<Run> SaveRunConfiguration(StartRunCommand cmd)
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
                        LogPath = null,
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