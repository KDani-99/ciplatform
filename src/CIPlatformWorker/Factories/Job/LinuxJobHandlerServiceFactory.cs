using System.Threading;
using CIPlatform.Data.Configuration;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using CIPlatformWorker.Services.Job;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.Factories.Job
{
    public class LinuxJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        public LinuxJobHandlerServiceFactory(IOptions<WorkerConfiguration> agentConfiguration)
            : base(agentConfiguration)
        {
        }

        public override IJobHandlerService Create(
                                                  JobConfiguration jobConfiguration,
                                                  CancellationToken cancellationToken = default)
        {
            return new LinuxJobHandlerService(jobConfiguration, AgentConfiguration, cancellationToken);
        }
    }
}