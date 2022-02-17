using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CIPlatformManager.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CIPlatformManager
{
    public class Manager : BackgroundService
    {
        private readonly IHost _host;
        private readonly ILogger<Manager> _logger;
        private readonly IManagerClient _workerClient;

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
                await ConnectWithRetryAsync(5, 15, stoppingToken);
                _logger.LogInformation("Connected to remote host.");
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception.Message);
                Environment.ExitCode = 1; // TODO: 
                await _host.StopAsync();
            }
        }
        
        private async Task ConnectWithRetryAsync(int retry, int delay, CancellationToken stoppingToken = default)
        {
            var retryCount = 0;

            Exception exception = null;
            
            while (retryCount < retry)
            {
                try
                {
                    await _workerClient.HubConnection.StartAsync(stoppingToken);
                    return;
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Failed to connect to remote host... (Attempt: {retryCount + 1}/{retry} | Delay: {delay} second(s))");
                    exception = e;
                }

                await Task.Delay(TimeSpan.FromSeconds(delay), stoppingToken);
                retryCount++;
            }

            throw exception;
        }
    }
}