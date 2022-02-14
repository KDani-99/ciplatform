using System;
using System.Runtime.InteropServices;
using CodeManager.Core.Hubs.Consumers;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Factories;
using CodeManagerAgent.WebSocket;
using CodeManagerAgent.WebSocket.Consumers;
using Docker.DotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeManagerAgent
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
                           var agentConfiguration = hostContext.Configuration.GetSection("AgentConfiguration")
                                                               .Get<AgentConfiguration>();
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

                           services.Configure<WebSocketConfiguration>(
                               hostContext.Configuration.GetSection("WebSocketConfiguration"));
                           services.Configure<AgentConfiguration>(
                               hostContext.Configuration.GetSection("AgentConfiguration"));
                           services.AddSingleton<IWorkerClient, WorkerClient>();
                           services.AddScoped<IDockerClient>(_ => new DockerClientConfiguration().CreateClient());
                           services.AddTransient<IConsumer<QueueJobCommand>, QueueJobCommandConsumer>();
                           services.AddHostedService<Worker>();
                           // transient services = 1 unit of work, each invocation returns a new service => like a factory
                           /*  services.AddTransient<IConsumer<QueueDockerJobEvent>, QueueDockerJobEventConsumer>();
                             services.AddTransient<IConsumer<QueueLinuxJobEvent>, QueueLinuxJobEventConsumer>();
                             services.AddTransient<IConsumer<QueueWindowsJobEvent>, QueueWindowsJobEventConsumer>();
                             services.AddRabbitMq(hostContext.Configuration);*/
                       });
        }

        private static void VerifyContext(AgentConfiguration agentConfiguration)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && agentConfiguration.Context != JobContext.Linux &&
                agentConfiguration.Context != JobContext.Docker)
                throw new InvalidOperationException(
                    "The agent context may only be either 'Docker' or 'Linux' on this machine");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                agentConfiguration.Context != JobContext.Windows && agentConfiguration.Context != JobContext.Docker)
                throw new InvalidOperationException(
                    "The agent context may only be either 'Docker' or 'Windows' on this machine");
        }
    }
}