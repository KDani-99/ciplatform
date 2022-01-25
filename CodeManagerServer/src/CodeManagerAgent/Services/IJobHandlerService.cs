using System;
using System.Threading.Tasks;

namespace CodeManagerAgent.Services
{
    public interface IJobHandlerService : IAsyncDisposable, IDisposable
    {
        public Task StartAsync();
    }
}