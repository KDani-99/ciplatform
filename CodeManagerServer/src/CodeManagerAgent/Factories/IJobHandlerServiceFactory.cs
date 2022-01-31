using System.Threading;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Services;

namespace CodeManagerAgent.Factories
{
    public interface IJobHandlerServiceFactory
    {
        public IJobHandlerService Create(JobDetails jobDetails, JobConfiguration jobConfiguration,
            CancellationToken cancellationToken = default);
    }
}