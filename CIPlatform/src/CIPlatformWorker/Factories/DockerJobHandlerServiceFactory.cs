﻿using System;
using System.Threading;
using CIPlatform.Data.Configuration;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using Docker.DotNet;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.Factories
{
    public class DockerJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        private readonly IDockerClient _dockerClient;

        public DockerJobHandlerServiceFactory(IDockerClient dockerClient,
                                              IOptions<WorkerConfiguration> agentConfiguration)
            : base(agentConfiguration)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }

        public override IJobHandlerService Create(JobConfiguration jobConfiguration,
                                                  CancellationToken cancellationToken)
        {

            return new DockerJobHandlerService(jobConfiguration, AgentConfiguration,
                _dockerClient, cancellationToken);
        }
    }
}