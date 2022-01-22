using System;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Configuration.StartJob;
using CodeManager.Data.Entities.CI;

namespace CodeManagerAgent.Services
{
    public interface IJobHandlerService : IAsyncDisposable, IDisposable
    {
        public Task StartAsync();
    }
}