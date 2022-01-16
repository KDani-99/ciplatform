using System;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Agent.Requests;
using CodeManager.Data.Agent.Responses;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Entities.CI;
using CodeManager.Data.Repositories;
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
        private readonly IRunRepository _runRepository;
        private readonly IRequestClient<AgentRequestRequest> _requestClient; // OBSERVER PATTERN
        private readonly ILogger<RunService> _logger;
        private readonly AgentManagerConfiguration _agentManagerConfiguration;

        private string _containerId;

        public RunService(IRunRepository runRepository, ILogger<RunService> logger, IOptions<AgentManagerConfiguration> agentManagerConfiguration)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _agentManagerConfiguration = agentManagerConfiguration.Value ??
                                         throw new ArgumentNullException(nameof(agentManagerConfiguration));
        }
        
        public async Task StartAsync(StartRunCommand cmd)
        {
            try
            {
                // wait for agent response, send broadcast
                var response = await _requestClient
                    .Create(new AgentRequestRequest(),
                        timeout: TimeSpan.FromSeconds(_agentManagerConfiguration.AgentRequestTimeoutInSeconds))
                    .GetResponse<AgentRequestResponse>();
                
                
                await SaveRunConfiguration(cmd);

            }
            catch (RequestTimeoutException exception)
            {
                // TODO:
            }
            catch (Exception exception)
            {

            }
        }
        

        private async Task SaveRunConfiguration(StartRunCommand cmd)
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
                    }).ToList()
                }).ToList(),
                ContextFilePath = cmd.ContextFilePath
            };
            await _runRepository.CreateAsync(runConfiguration);
        }
    }
}