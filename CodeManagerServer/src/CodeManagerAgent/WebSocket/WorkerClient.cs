using System;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Clients;
using CodeManager.Core.Hubs.Consumers;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManagerAgent.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.WebSocket
{
    public class WorkerClient : IWorkerClient, IDisposable
    {
        // TODO: do the same with this class as I did with redis, register this class as singleton
        public HubConnection HubConnection { get; }
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WorkerClient> _logger;

        public WorkerClient(IServiceProvider serviceProvider, IOptions<WebSocketConfiguration> webSocketConfiguration, ILogger<WorkerClient> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var wsConfiguration = webSocketConfiguration.Value ?? throw new ArgumentNullException(nameof(webSocketConfiguration));
            
            HubConnection = new HubConnectionBuilder()
                .WithUrl($"{wsConfiguration.Host}/{wsConfiguration.Hub}",
                    options => options.AccessTokenProvider = () => Task.FromResult(""))
                .Build();

            HubConnection.Reconnecting += OnReconnection;
            HubConnection.Reconnected += OnReconnected;
            HubConnection.Closed += OnConnectionClose;
            
            RegisterMethods();
        }

        private void RegisterMethods()
        {
            _logger.LogInformation("Registering worker events...");
            HubConnection.On<QueueDockerJobEvent>(CommonHubMethods.QueueDockerJobEvent,
                message => _serviceProvider.GetService<IConsumer<QueueDockerJobEvent>>()?.Consume(message));
            HubConnection.On<QueueLinuxJobEvent>(CommonHubMethods.QueueLinuxJobEvent,
                message => _serviceProvider.GetService<IConsumer<QueueLinuxJobEvent>>()?.Consume(message));
            HubConnection.On<QueueWindowsJobEvent>(CommonHubMethods.QueueWindowsJobEvent,
                message => _serviceProvider.GetService<IConsumer<QueueWindowsJobEvent>>()?.Consume(message));
        }

        private Task OnConnectionClose(Exception exception)
        {
            _logger.LogError($"Connection lost. Error: " + exception.Message);
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
        
        public void Dispose()
        {
            HubConnection.Reconnected -= OnReconnected;
            HubConnection.Reconnecting -= OnReconnection;
            HubConnection.Closed -= OnConnectionClose;
            HubConnection?.DisposeAsync();
        }
    }
}