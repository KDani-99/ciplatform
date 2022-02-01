using System;
using System.Threading;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Services;
using CodeManagerAgent.WebSocket;
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

        public DockerJobHandlerServiceFactory(IDockerClient dockerClient, IOptions<AgentConfiguration> agentConfiguration)
            : base(agentConfiguration)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }

        public override IJobHandlerService Create(JobDetails jobDetails, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken)
        {
            return new DockerJobHandlerService(jobDetails, jobConfiguration, AgentConfiguration,
                _dockerClient, cancellationToken);
        }
    }
}