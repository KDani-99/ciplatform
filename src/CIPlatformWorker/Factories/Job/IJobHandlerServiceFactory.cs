using System.Threading;
using CIPlatform.Data.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using CIPlatformWorker.Services.Job;

namespace CIPlatformWorker.Factories.Job
{
    public interface IJobHandlerServiceFactory
    {
        public IJobHandlerService Create(
                                         JobConfiguration jobConfiguration,
                                         CancellationToken cancellationToken = default);
    }
}