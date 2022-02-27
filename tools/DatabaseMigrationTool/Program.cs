using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIPlatformWebApi.Database;
using CIPlatformWebApi.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DatabaseMigrationTool
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    services.AddDbContext<CIPlatformDbContext>(options =>
                        options.UseNpgsql(
                            configuration.GetValue<string>("ConnectionString"), x =>
                            {
                                x.MigrationsAssembly(nameof(DatabaseMigrationTool));
                            }));

                    // register the required services
                    services.AddScoped<ICredentialManagerService, CredentialManagerService>();

                    services.AddHostedService<MigrationHelper>();
                });
        }
    }
}