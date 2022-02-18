using System;
using System.Runtime.InteropServices;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.WebSocket;
using CIPlatformWorker.WebSocket.Consumers;
using CIPlatform.Core.Hubs.Consumers;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using CIPlatformWorker.Factories;
using CIPlatformWorker.Factories.Job;
using CIPlatformWorker.Services;
using Docker.DotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CIPlatformWorker
{
    public class Program
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
                           var agentConfiguration = hostContext.Configuration.GetSection("WorkerConfiguration")
                                                               .Get<WorkerConfiguration>();
                           VerifyContext(agentConfiguration);

                           switch (agentConfiguration.Context)
                           {
                               case JobContext.Linux:
                                   services.AddScoped<IJobHandlerServiceFactory, LinuxJobHandlerServiceFactory>();
                                   break;
                               case JobContext.Windows:
                                   services.AddScoped<IJobHandlerServiceFactory, WindowsJobHandlerServiceFactory>();
                                   break;
                               case JobContext.Docker:
                                   services.AddScoped<IJobHandlerServiceFactory, DockerJobHandlerServiceFactory>();
                                   break;
                               default:
                                   throw new ArgumentOutOfRangeException(
                                       nameof(agentConfiguration.Context));
                           }

                           services
                               .Configure<WebSocketConfiguration>(
                               hostContext.Configuration.GetSection("WebSocketConfiguration"))
                               .Configure<WorkerConfiguration>(
                               hostContext.Configuration.GetSection("WorkerConfiguration"));
                           
                           services.AddSingleton<IWorkerClient, WorkerClient>();
                           services.AddScoped<IDockerClient>(_ => new DockerClientConfiguration().CreateClient());
                           services.AddTransient<IConsumer<QueueJobCommand>, QueueJobCommandConsumer>();

                           services.AddHostedService<KeepAliveService>();
                           services.AddHostedService<Worker>();
                       });
        }

        private static void VerifyContext(WorkerConfiguration workerConfiguration)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && workerConfiguration.Context != JobContext.Linux &&
                workerConfiguration.Context != JobContext.Docker)
                throw new InvalidOperationException(
                    "The agent context may only be either 'Docker' or 'Linux' on this machine");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                workerConfiguration.Context != JobContext.Windows && workerConfiguration.Context != JobContext.Docker)
                throw new InvalidOperationException(
                    "The agent context may only be either 'Docker' or 'Windows' on this machine");
        }
    }
}