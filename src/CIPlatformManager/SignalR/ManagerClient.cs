using System;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CIPlatformManager.SignalR
{
    public class ManagerClient : IManagerClient, IDisposable
    {
        private readonly ILogger<ManagerClient> _logger;

        public ManagerClient(IOptions<SignalRConfiguration> webSocketConfiguration, ILogger<ManagerClient> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var wsConfiguration = webSocketConfiguration.Value ??
                throw new ArgumentNullException(nameof(webSocketConfiguration));

            HubConnection = new HubConnectionBuilder()
                            .WithUrl($"{wsConfiguration.Host}/{wsConfiguration.Hub}",
                                     options => { options.AccessTokenProvider = () => Task.FromResult(""); })
                            .WithAutomaticReconnect()
                            .Build();

            HubConnection.Reconnecting += OnReconnection;
            HubConnection.Reconnected += OnReconnected;
            HubConnection.Closed += OnConnectionClose;

            HubConnection.ServerTimeout = TimeSpan.FromSeconds(180);
            HubConnection.HandshakeTimeout = TimeSpan.FromSeconds(60);
        }

        public void Dispose()
        {
            HubConnection.Reconnected -= OnReconnected;
            HubConnection.Reconnecting -= OnReconnection;
            HubConnection.Closed -= OnConnectionClose;
            HubConnection?.DisposeAsync();
        }
        
        public HubConnection HubConnection { get; }
        
        private Task OnConnectionClose(Exception exception)
        {
            _logger.LogError("Connection lost. Error: " + exception.Message);
            return Task.CompletedTask;
        }

        private Task OnReconnection(Exception _)
        {
            _logger.LogWarning("Reconnecting...");
            return Task.CompletedTask;
        }

        private Task OnReconnected(string _)
        {
            _logger.LogError("Reconnected to remote host.");
            return Task.CompletedTask;
        }
    }
}