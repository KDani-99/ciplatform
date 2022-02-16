using System.Threading;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using CIPlatform.Data.Configuration;

namespace CIPlatformWorker.Factories
{
    public interface IJobHandlerServiceFactory
    {
        public IJobHandlerService Create(JobDetails jobDetails,
                                         JobConfiguration jobConfiguration,
                                         CancellationToken cancellationToken = default);
    }
}