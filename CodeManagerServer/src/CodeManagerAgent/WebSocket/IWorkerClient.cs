using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerAgent.WebSocket
{
    public interface IWorkerClient
    {
        public HubConnection HubConnection { get; }
    }
}