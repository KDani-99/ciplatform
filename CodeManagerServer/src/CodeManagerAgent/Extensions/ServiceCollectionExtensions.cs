﻿using System;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeManagerAgent.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddMassTransit(busConfigurator =>
            {
                var massTransitConfiguration =
                    configuration.GetSection("MassTransitConfiguration").Get<MassTransitConfiguration>();
                var agentConfiguration = configuration.GetSection("AgentConfiguration").Get<AgentConfiguration>();
                
                busConfigurator.AddRequestClient<RequestJobCommand>();
                switch (agentConfiguration.Context)
                {
                    case JobContext.Docker:
                        busConfigurator.AddConsumer<QueueDockerJobEventConsumer>();
                        break;
                    case JobContext.Linux:
                        busConfigurator.AddConsumer<QueueLinuxJobEventConsumer>();
                        break;
                    case JobContext.Windows:
                        busConfigurator.AddConsumer<QueueWindowsJobEventConsumer>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            "The agent context must be one of the following types: 'Linux', 'Windows', 'Docker'");
                }

                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(massTransitConfiguration.Host, massTransitConfiguration.VirtualHost,
                        h =>
                        {
                            h.Username(massTransitConfiguration.Username);
                            h.Password(massTransitConfiguration.Password);
                        });

                    cfg.ReceiveEndpoint(
                        $"{massTransitConfiguration.Queues["QueueJobQueue"]}-{Guid.NewGuid()}", // all agents should receive it -> competing consumer avoided
                        opts =>
                        {
                            opts.AutoDelete = true;
                            switch (agentConfiguration.Context)
                            {
                                case JobContext.Docker:
                                    opts.ConfigureConsumer<QueueDockerJobEventConsumer>(context);
                                    break;
                                case JobContext.Linux:
                                    opts.ConfigureConsumer<QueueLinuxJobEventConsumer>(context);
                                    break;
                                case JobContext.Windows:
                                    opts.ConfigureConsumer<QueueWindowsJobEventConsumer>(context);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(
                                        "The agent context must be one of the following types: 'Linux', 'Windows', 'Docker'");
                            }
                        });
                });

                services.AddMassTransitHostedService(true);
            });
        }
    }
}