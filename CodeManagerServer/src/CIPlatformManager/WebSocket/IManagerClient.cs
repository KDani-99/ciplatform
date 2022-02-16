using Microsoft.AspNetCore.SignalR.Client;

namespace CIPlatformManager.WebSocket
{
    public interface IManagerClient
    {
        public HubConnection HubConnection { get; }
    }
}