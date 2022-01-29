using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerAgentManager.WebSocket
{
    public interface IManagerClient
    {
        public HubConnection HubConnection { get; }
    }
}