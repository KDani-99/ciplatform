using System;
using System.Threading;
using System.Threading.Tasks;
using CIPlatformWorker.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CIPlatformWorker.Services
{
    public class KeepAliveService : BackgroundService
    {
        private readonly IWorkerClient _workerClient;
        private readonly ILogger<KeepAliveService> _logger;

        public KeepAliveService(IWorkerClient workerClient, ILogger<KeepAliveService> logger)
        {
            _workerClient = workerClient ?? throw new ArgumentNullException(nameof(workerClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_workerClient.HubConnection.State == HubConnectionState.Connected)
                    {
                        await _workerClient.PingAsync();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"An unexpected error has occured. Message: {exception.Message}");
            }
            
        }
    }
}