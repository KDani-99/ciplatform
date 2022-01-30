using System;
using System.Threading;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
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
        protected readonly IAgentService AgentService;
        protected readonly IWorkerClient WorkerClient;
        protected readonly ILoggerFactory LoggerFactory;

        protected JobHandlerServiceFactory(IWorkerClient workerClient, IAgentService agentService, IOptions<AgentConfiguration> agentConfiguration, ILoggerFactory loggerFactory)
        {
            WorkerClient = workerClient ?? throw new ArgumentNullException(nameof(workerClient));
            AgentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            AgentConfiguration = agentConfiguration ?? throw new ArgumentNullException(nameof(agentConfiguration));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public abstract IJobHandlerService Create(string repository, string token, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken = default);
    }
}