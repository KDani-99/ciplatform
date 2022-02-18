using System;
using System.Threading;
using CIPlatform.Data.Configuration;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.Factories
{
    public abstract class JobHandlerServiceFactory : IJobHandlerServiceFactory
    {
        protected readonly IOptions<WorkerConfiguration> AgentConfiguration;

        protected JobHandlerServiceFactory(IOptions<WorkerConfiguration> agentConfiguration)
        {
            AgentConfiguration = agentConfiguration ?? throw new ArgumentNullException(nameof(agentConfiguration));
        }

        public abstract IJobHandlerService Create(JobConfiguration jobConfiguration,
                                                  CancellationToken cancellationToken = default);
    }
}