using System.Threading;
using CIPlatform.Data.Configuration;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using CIPlatformWorker.Services.Job;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.Factories.Job
{
    public class WindowsJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        public WindowsJobHandlerServiceFactory(IOptions<WorkerConfiguration> agentConfiguration)
            : base(agentConfiguration)
        {
        }

        public override IJobHandlerService Create(
                                                  JobConfiguration jobConfiguration,
                                                  CancellationToken cancellationToken)
        {
            return new WindowsJobHandlerService(jobConfiguration, AgentConfiguration, cancellationToken);
        }
    }
}