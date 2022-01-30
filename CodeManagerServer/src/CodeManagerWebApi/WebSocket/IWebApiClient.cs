using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerWebApi.WebSocket
{
    public interface IWebApiClient
    {
        public HubConnection HubConnection { get; }
    }
}