using System.Threading;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using CIPlatform.Data.Configuration;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.Factories
{
    public class WindowsJobHandlerServiceFactory : JobHandlerServiceFactory
    {
        public WindowsJobHandlerServiceFactory(IOptions<AgentConfiguration> agentConfiguration)
            : base(agentConfiguration)
        {
        }

        public override IJobHandlerService Create(JobDetails jobDetails,
                                                  JobConfiguration jobConfiguration,
                                                  CancellationToken cancellationToken)
        {
            return new WindowsJobHandlerService(jobDetails, jobConfiguration, AgentConfiguration, cancellationToken);
        }
    }
}