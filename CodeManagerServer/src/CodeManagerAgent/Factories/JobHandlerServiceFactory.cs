using System;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.Factories
{
    public abstract class JobHandlerServiceFactory : IJobHandlerServiceFactory
    {
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly IAgentService AgentService;
        protected readonly IBusControl BusControl;
        
        protected JobHandlerServiceFactory(ILoggerFactory loggerFactory, IAgentService agentService, IBusControl busControl)
        {
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            AgentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            BusControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
        }
        
        public abstract IJobHandlerService Create(string token, JobConfiguration jobConfiguration, Uri responseAddress);
    }
}