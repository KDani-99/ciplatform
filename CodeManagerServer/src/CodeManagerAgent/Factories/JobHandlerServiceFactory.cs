using System;
using System.Threading;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Factories
{
    public abstract class JobHandlerServiceFactory : IJobHandlerServiceFactory
    {
        protected readonly IAgentService AgentService;
        protected readonly IBusControl BusControl;
        protected readonly IOptions<AgentConfiguration> AgentConfiguration;
        
        protected JobHandlerServiceFactory(IAgentService agentService, IBusControl busControl, IOptions<AgentConfiguration> agentConfiguration)
        {
            AgentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            BusControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            AgentConfiguration = agentConfiguration ?? throw new ArgumentNullException(nameof(agentConfiguration));
        }
        
        public abstract IJobHandlerService Create(string token, JobConfiguration jobConfiguration, CancellationToken cancellationToken);
    }
}