using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CIPlatformWorker.SignalR;
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
                await ConnectWithRetryAsync(5, 15, stoppingToken);
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