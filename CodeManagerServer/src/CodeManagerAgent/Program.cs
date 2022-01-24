using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Extensions;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Hubs.Consumers;
using CodeManagerAgent.Services;
using Docker.DotNet;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using HubConnection = Microsoft.AspNet.SignalR.Client.HubConnection;

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
                                "The agent context must be one of the following types: 'Linux', 'Windows', 'Docker'");
                    }

                    services.Configure<AgentConfiguration>(hostContext.Configuration.GetSection("AgentConfiguration"));
                    services.AddSingleton<IAgentService, AgentService>();
                    services.AddScoped<IDockerClient>(_ => new DockerClientConfiguration().CreateClient());
                    services.AddSignalRClient(hostContext.Configuration);
                    services.AddTransient<IConsumer<QueueDockerJobEvent>, QueueDockerJobEventConsumer>();
                    services.AddTransient<IConsumer<QueueLinuxJobEvent>, QueueLinuxJobEventConsumer>();
                    services.AddTransient<IConsumer<QueueWindowsJobEvent>, QueueWindowsJobEventConsumer>();
                    services.AddRabbitMq(hostContext.Configuration);
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