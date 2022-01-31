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
        protected readonly IWorkerClient WorkerClient;
        protected readonly ILoggerFactory LoggerFactory;

        protected JobHandlerServiceFactory(IWorkerClient workerClient, IOptions<AgentConfiguration> agentConfiguration, ILoggerFactory loggerFactory)
        {
            WorkerClient = workerClient ?? throw new ArgumentNullException(nameof(workerClient));
            AgentConfiguration = agentConfiguration ?? throw new ArgumentNullException(nameof(agentConfiguration));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public abstract IJobHandlerService Create(JobDetails jobDetails, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken = default);
    }
}