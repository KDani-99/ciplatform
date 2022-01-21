using System;
using System.Threading;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Services;
using Docker.DotNet;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Factories
{
    public class DockerJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        private readonly IDockerClient _dockerClient;

        public DockerJobHandlerServiceFactory(IDockerClient dockerClient, IAgentService agentService, IBusControl busControl, IOptions<AgentConfiguration> agentConfiguration)
            : base(agentService, busControl, agentConfiguration)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }
        
        public override IJobHandlerService Create(string token, JobConfiguration jobConfiguration, CancellationToken cancellationToken)
        {
            return new DockerJobHandlerService(token, jobConfiguration, AgentConfiguration,_dockerClient, BusControl,
                AgentService, cancellationToken);
        }
    }
}