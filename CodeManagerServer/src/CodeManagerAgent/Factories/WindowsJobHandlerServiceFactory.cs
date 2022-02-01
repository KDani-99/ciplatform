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
    public class WindowsJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        public WindowsJobHandlerServiceFactory(IOptions<AgentConfiguration> agentConfiguration)
            : base(agentConfiguration)
        {
        }

        public override IJobHandlerService Create(JobDetails jobDetails, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken)
        {
            return new WindowsJobHandlerService(jobDetails, jobConfiguration, AgentConfiguration, cancellationToken);
        }
    }
}