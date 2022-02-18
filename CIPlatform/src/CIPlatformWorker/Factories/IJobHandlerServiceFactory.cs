using System.Threading;
using CIPlatform.Data.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;

namespace CIPlatformWorker.Factories
{
    public interface IJobHandlerServiceFactory
    {
        public IJobHandlerService Create(
                                         JobConfiguration jobConfiguration,
                                         CancellationToken cancellationToken = default);
    }
}