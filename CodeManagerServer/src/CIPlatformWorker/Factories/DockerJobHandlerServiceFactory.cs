using System;
using System.Threading;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using CIPlatform.Data.Configuration;
using Docker.DotNet;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.Factories
{
    public class DockerJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        private readonly IDockerClient _dockerClient;

        public DockerJobHandlerServiceFactory(IDockerClient dockerClient,
                                              IOptions<AgentConfiguration> agentConfiguration)
            : base(agentConfiguration)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }

        public override IJobHandlerService Create(JobDetails jobDetails,
                                                  JobConfiguration jobConfiguration,
                                                  CancellationToken cancellationToken)
        {
            return new DockerJobHandlerService(jobDetails, jobConfiguration, AgentConfiguration,
                                               _dockerClient, cancellationToken);
        }
    }
}