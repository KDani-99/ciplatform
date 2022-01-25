using System;
using System.Threading;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Services;
using Docker.DotNet;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Factories
{
    public class DockerJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        private readonly IDockerClient _dockerClient;

        public DockerJobHandlerServiceFactory(HubConnection hubConnection, IDockerClient dockerClient,
            IAgentService agentService, IBusControl busControl, IOptions<AgentConfiguration> agentConfiguration,
            ILoggerFactory loggerFactory)
            : base(hubConnection, agentService, busControl, agentConfiguration, loggerFactory)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }

        public override IJobHandlerService Create(string repository, string token, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken)
        {
            return new DockerJobHandlerService(repository, token, jobConfiguration, HubConnection, AgentConfiguration,
                _dockerClient, BusControl,
                AgentService, LoggerFactory.CreateLogger<JobHandlerService>(), cancellationToken);
        }
    }
}