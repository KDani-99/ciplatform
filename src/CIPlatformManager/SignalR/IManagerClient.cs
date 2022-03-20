using Microsoft.AspNetCore.SignalR.Client;

namespace CIPlatformManager.SignalR
{
    public interface IManagerClient
    {
        public HubConnection HubConnection { get; }
    }
}