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

        public DockerJobHandlerServiceFactory(IWorkerClient workerClient, IDockerClient dockerClient, IOptions<AgentConfiguration> agentConfiguration, ILoggerFactory loggerFactory)
            : base(workerClient, agentConfiguration, loggerFactory)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }

        public override IJobHandlerService Create(JobDetails jobDetails, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken)
        {
            return new DockerJobHandlerService(jobDetails, jobConfiguration, WorkerClient, AgentConfiguration,
                _dockerClient, LoggerFactory.CreateLogger<JobHandlerService>(), cancellationToken);
        }
    }
}