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
    public class LinuxJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        public LinuxJobHandlerServiceFactory(HubConnection hubConnection, IAgentService agentService,
            IBusControl busControl, IOptions<AgentConfiguration> agentConfiguration, ILoggerFactory loggerFactory)
            : base(hubConnection, agentService, busControl, agentConfiguration, loggerFactory)
        {
        }

        public override IJobHandlerService Create(string repository, string token, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken)
        {
            return new LinuxJobHandlerService(repository, token, jobConfiguration, HubConnection, AgentConfiguration,
                BusControl,
                AgentService, LoggerFactory.CreateLogger<JobHandlerService>(), cancellationToken);
        }
    }
}