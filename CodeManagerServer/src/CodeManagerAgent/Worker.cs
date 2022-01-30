using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeManagerAgent.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerAgent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWorkerClient _workerClient;
        private readonly IHost _host;

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
                Environment.ExitCode = 1; // TODO: 
                await _host.StopAsync();
            }
        }
    }
}