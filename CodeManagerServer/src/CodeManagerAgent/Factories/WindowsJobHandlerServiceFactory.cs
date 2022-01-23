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
    public class WindowsJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        public WindowsJobHandlerServiceFactory(IAgentService agentService, IBusControl busControl, IOptions<AgentConfiguration> agentConfiguration)
            : base(agentService, busControl, agentConfiguration)
        {
        }
        
        public override IJobHandlerService Create(string repository, string token, JobConfiguration jobConfiguration, CancellationToken cancellationToken)
        {
            return new WindowsJobHandlerService(repository, token, jobConfiguration, AgentConfiguration, BusControl,
                AgentService, cancellationToken);
        }
    }
}