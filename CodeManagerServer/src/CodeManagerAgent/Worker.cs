using System;
using System.Threading;
using System.Threading.Tasks;
using CodeManagerAgent.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWorkerClient _workerClient;

        public Worker(ILogger<Worker> logger, IWorkerClient workerClient)
        {
            _logger = logger;
            _workerClient = workerClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting worker...");
            await _workerClient.HubConnection.StartAsync(stoppingToken);
        }
    }
}