using System;
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
        
        public override IJobHandlerService Create(string token, JobConfiguration jobConfiguration, Uri responseAddress)
        {
            return new WindowsJobHandlerService(token, jobConfiguration, responseAddress, AgentConfiguration, BusControl,
                AgentService);
        }
    }
}