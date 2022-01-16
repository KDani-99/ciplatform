using System;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Services;
using Docker.DotNet;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.Factories
{
    public class DockerJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        private readonly IDockerClient _dockerClient;

        public DockerJobHandlerServiceFactory(IDockerClient dockerClient, ILoggerFactory loggerFactory, IAgentService agentService, IBusControl busControl)
            : base(loggerFactory, agentService, busControl)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }
        
        public override IJobHandlerService Create(string token, JobConfiguration jobConfiguration, Uri responseAddress)
        {
            return new DockerJobHandlerService(token, jobConfiguration, responseAddress, _dockerClient, LoggerFactory.CreateLogger<DockerJobHandlerService>(), BusControl,
                AgentService);
        }
    }
}