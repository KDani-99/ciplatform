using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CIPlatformWebApi.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DatabaseMigrationTool
{
    public class MigrationHelper : BackgroundService
    {
        private readonly ILogger<MigrationHelper> _logger;
        private readonly IServiceProvider _serviceProvider;

        public MigrationHelper(ILogger<MigrationHelper> logger, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(TimeSpan.FromSeconds(60));

                _logger.LogInformation("Starting DbContext migration...");

                await using var dbContext = _serviceProvider.GetService<CIPlatformDbContext>();
                await dbContext!.Database.MigrateAsync(tokenSource.Token);

                _logger.LogInformation("DbContext migration has succeeded!");
            }
            catch (OperationCanceledException exception)
            {
                _logger.LogError("Failed to apply database migration. Migration timed out.");
                Environment.Exit(exception.HResult);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "An error has occured.");
                Environment.Exit(exception.HResult);
                throw;
            }

            Environment.Exit(0);
        }
    }
}