using System;
using CodeManager.Data.Configuration;
using CodeManagerAgentManager.Consumers.RabbitMq;
using MassTransit;
using MassTransit.JobService;
using MassTransit.JobService.Components.StateMachines;
using MassTransit.RabbitMqTransport.Topology.Entities;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeManagerAgentManager.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddMassTransit(busConfigurator =>
            {
                var massTransitConfiguration =
                    configuration.GetSection("MassTransitConfiguration").Get<MassTransitConfiguration>();

                busConfigurator.AddConsumer<QueueRunCommandConsumer>();
                busConfigurator.AddConsumer<StepResultEventConsumer>();

                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(massTransitConfiguration.Host, massTransitConfiguration.VirtualHost,
                        h =>
                        {
                            h.Username(massTransitConfiguration.Username);
                            h.Password(massTransitConfiguration.Password);
                        });

                    cfg.ReceiveEndpoint(massTransitConfiguration.Queues["QueueRunCommandQueue"], opts =>
                    {
                        opts.AutoDelete = true;
                        opts.ConfigureConsumer<QueueRunCommandConsumer>(context);
                        //opts.SingleActiveConsumer
                    });
                    cfg.ReceiveEndpoint(massTransitConfiguration.Queues["StepResultEventQueue"],
                        opts =>
                        {
                            opts.AutoDelete = true;
                            opts.ConfigureConsumer<StepResultEventConsumer>(context);
                        });
                });

                services.AddMassTransitHostedService(true);
            });
        }
    }
}