using System;
using System.Threading;
using System.Threading.Tasks;
using CodeManagerAgentManager.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgentManager
{
    public class Manager : BackgroundService
    {
        private readonly ILogger<Manager> _logger;
        private readonly IManagerClient _workerClient;
        private readonly IHost _host;

        public Manager(ILogger<Manager> logger, IManagerClient workerClient, IHost host)
        {
            _logger = logger;
            _workerClient = workerClient;
            _host = host;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting manager...");
                await _workerClient.HubConnection.StartAsync(stoppingToken);
                _logger.LogInformation("Connected to remote host.");
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception.Message);
                Environment.ExitCode = 1; // TODO: 
                await _host.StopAsync();
            }
        }
    }
}