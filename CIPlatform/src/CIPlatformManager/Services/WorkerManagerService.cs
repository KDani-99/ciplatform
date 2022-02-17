﻿using System;
using System.Threading;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatformManager.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CIPlatformManager.Services
{
    public class WorkerManagerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WorkerManagerService> _logger;
        
        public WorkerManagerService(IServiceProvider serviceProvider, ILogger<WorkerManagerService> logger)
        {
            _serviceProvider = serviceProvider ??
                throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task RemoveUnknownWorkersOfTypeAsync(JobContext jobContext)
        {

            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetService<IWorkerConnectionService>();
            
            foreach (var connectionId in await service!.GetAvailableWorkerConnectionIdsOfTypeAsync(jobContext))
            {

                var workerConnectionData = await service.GetWorkerConnectionAsync(connectionId);

                if (!HasExpired(workerConnectionData.LastPing))
                {
                    continue;
                }

                await service.RemoveWorkerConnectionAsync(connectionId);
                
                _logger.LogInformation($"An unknown worker has been removed @ {DateTime.Now} (connection id: {connectionId}).");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting unknown worker cleanup...");
                
                await RemoveUnknownWorkersOfTypeAsync(JobContext.Docker);
                await RemoveUnknownWorkersOfTypeAsync(JobContext.Linux);
                await RemoveUnknownWorkersOfTypeAsync(JobContext.Windows);
                
                _logger.LogInformation("Finished unknown worker cleanup.");

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }

        private static bool HasExpired(DateTime time)
        {
            return DateTime.Now - time > TimeSpan.FromSeconds(60);
        }
    }
}