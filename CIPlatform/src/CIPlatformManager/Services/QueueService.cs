using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Events;
using CIPlatformManager.Exceptions;
using CIPlatformManager.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;

namespace CIPlatformManager.Services
{
    public class QueueService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QueueService> _logger;

        public QueueService(IServiceProvider serviceProvider, ILogger<QueueService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await TryQueueJobAsync(JobContext.Docker);
                    await TryQueueJobAsync(JobContext.Linux);
                    await TryQueueJobAsync(JobContext.Windows);
                }
                catch (Exception exception)
                {
                    _logger.LogError($"An error has occured while trying to queue a job. Message: {exception.Message}");
                }
                
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }

        private async Task TryQueueJobAsync(JobContext jobContext)
        {
            using var scope = _serviceProvider.CreateScope();
            
            var jobQueueService = scope.ServiceProvider.GetService<IJobQueueService>();
            var jobService = scope.ServiceProvider.GetService<IJobService>();
            var workerService = scope.ServiceProvider.GetService<IWorkerConnectionService>();

            var job = await jobQueueService!.DequeueAsync(jobContext);

            if (job is null)
            {
                _logger.LogDebug($"Job queue was empty (type: {jobContext.GetDisplayName()}).");
                return;
            }
            
            // Removing the worker from the queue makes it race condition proof
            var connectionId = await workerService!.DequeueAvailableWorkerConnectionOfTypeAsync(jobContext);
            
            if (connectionId is null)
            {
                // Add back into the queue if there are no workers available
                await jobQueueService.AddFrontAsync(job, jobContext);
                _logger.LogWarning($"No worker could be found of type: {jobContext.GetDisplayName()}. Job was put back to the queue (id: {job.JobId}).");
                return;
            }

            await jobService!.QueueJobAsync(job, connectionId);
            _logger.LogInformation($"Job was sent to worker (connection id: {connectionId} | type: {jobContext.GetDisplayName()}).");
        }
    }
}