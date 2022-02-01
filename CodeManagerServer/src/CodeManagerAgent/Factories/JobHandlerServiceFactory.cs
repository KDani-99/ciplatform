using System;
using System.Threading;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Services;
using CodeManagerAgent.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Factories
{
    public abstract class JobHandlerServiceFactory : IJobHandlerServiceFactory
    {
        protected readonly IOptions<AgentConfiguration> AgentConfiguration;

        protected JobHandlerServiceFactory(IOptions<AgentConfiguration> agentConfiguration)
        {
            AgentConfiguration = agentConfiguration ?? throw new ArgumentNullException(nameof(agentConfiguration));
        }

        public abstract IJobHandlerService Create(JobDetails jobDetails, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken = default);
    }
}