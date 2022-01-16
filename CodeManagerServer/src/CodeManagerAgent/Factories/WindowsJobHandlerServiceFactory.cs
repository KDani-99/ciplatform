using System;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.Factories
{
    public class WindowsJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        public WindowsJobHandlerServiceFactory(ILoggerFactory loggerFactory, IAgentService agentService, IBusControl busControl)
            : base(loggerFactory, agentService, busControl)
        {
        }
        
        public override IJobHandlerService Create(string token, JobConfiguration jobConfiguration, Uri responseAddress)
        {
            return new WindowsJobHandlerService(token, jobConfiguration, responseAddress, LoggerFactory.CreateLogger<DockerJobHandlerService>(), BusControl,
                AgentService);
        }
    }
}