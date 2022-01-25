using System;
using System.Threading;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Services;
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
        protected readonly IBusControl BusControl;
        protected readonly HubConnection HubConnection;
        protected readonly ILoggerFactory LoggerFactory;

        protected JobHandlerServiceFactory(HubConnection hubConnection, IAgentService agentService,
            IBusControl busControl, IOptions<AgentConfiguration> agentConfiguration, ILoggerFactory loggerFactory)
        {
            HubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));
            AgentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            BusControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            AgentConfiguration = agentConfiguration ?? throw new ArgumentNullException(nameof(agentConfiguration));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public abstract IJobHandlerService Create(string repository, string token, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken);
    }
}