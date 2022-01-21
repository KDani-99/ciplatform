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
    public class LinuxJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        public LinuxJobHandlerServiceFactory(IAgentService agentService, IBusControl busControl, IOptions<AgentConfiguration> agentConfiguration)
            : base(agentService, busControl, agentConfiguration)
        {
        }
        
        public override IJobHandlerService Create(string token, JobConfiguration jobConfiguration, CancellationToken cancellationToken)
        {
            return new LinuxJobHandlerService(token, jobConfiguration, AgentConfiguration, BusControl,
                AgentService, cancellationToken);
        }
    }
}