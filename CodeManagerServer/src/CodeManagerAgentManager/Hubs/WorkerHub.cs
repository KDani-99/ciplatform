using System;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Clients;
using CodeManager.Core.Hubs.Messages;
using CodeManager.Data.Entities.CI;
using CodeManager.Data.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace CodeManagerAgent.Hubs
{
    public class WorkerHub : Hub<IWorkerClient>
    {
        private readonly IRunRepository _runRepository;

        public WorkerHub(IRunRepository runRepository)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("log")]
        public async Task LogAsync(LogMessage logMessage)
        {
            
        }
        
    }
}