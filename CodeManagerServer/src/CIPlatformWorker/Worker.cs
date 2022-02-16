using System;
using System.Threading;
using System.Threading.Tasks;
using CIPlatformWorker.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CIPlatformWorker
{
    public class Worker : BackgroundService
    {
        private readonly IHost _host;
        private readonly ILogger<Worker> _logger;
        private readonly IWorkerClient _workerClient;

        public Worker(ILogger<Worker> logger, IWorkerClient workerClient, IHost host)
        {
            _logger = logger;
            _workerClient = workerClient;
            _host = host;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting worker...");
                await _workerClient.HubConnection.StartAsync(stoppingToken);
                _logger.LogInformation("Connected to remote host.");
                await _workerClient.ConfigureAsync();
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception.Message);
                Environment.ExitCode = 1;
                await _host.StopAsync();
            }
        }
    }
}