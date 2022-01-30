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
    public class WindowsJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        public WindowsJobHandlerServiceFactory(IWorkerClient workerClient, IAgentService agentService, IOptions<AgentConfiguration> agentConfiguration, ILoggerFactory loggerFactory)
            : base(workerClient, agentService, agentConfiguration, loggerFactory)
        {
        }

        public override IJobHandlerService Create(string repository, string token, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken)
        {
            return new WindowsJobHandlerService(repository, token, jobConfiguration, WorkerClient, AgentConfiguration, LoggerFactory.CreateLogger<JobHandlerService>(), cancellationToken);
        }
    }
}