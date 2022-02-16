using System;
using System.Threading;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using CIPlatform.Data.Configuration;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.Factories
{
    public abstract class JobHandlerServiceFactory : IJobHandlerServiceFactory
    {
        protected readonly IOptions<AgentConfiguration> AgentConfiguration;

        protected JobHandlerServiceFactory(IOptions<AgentConfiguration> agentConfiguration)
        {
            AgentConfiguration = agentConfiguration ?? throw new ArgumentNullException(nameof(agentConfiguration));
        }

        public abstract IJobHandlerService Create(JobDetails jobDetails,
                                                  JobConfiguration jobConfiguration,
                                                  CancellationToken cancellationToken = default);
    }
}