using System;
using CodeManager.Data.Agent.Requests;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Consumers;
using MassTransit;
using MassTransit.JobService;
using MassTransit.JobService.Components.StateMachines;
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
                var massTransitConfiguration = configuration.GetSection("MassTransitConfiguration").Get<MassTransitConfiguration>();

                busConfigurator.AddConsumer<QueueJobEventConsumer>();
                busConfigurator.AddRequestClient<RequestJobCommand>();
                
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(massTransitConfiguration.Host, massTransitConfiguration.VirtualHost,
                        h =>
                        {
                            h.Username(massTransitConfiguration.Username);
                            h.Password(massTransitConfiguration.Password);
                        });
                    
                    cfg.ReceiveEndpoint($"{massTransitConfiguration.Queues["QueueJobQueue"]}-{Guid.NewGuid()}", // all agents should receive it -> competing consumer avoided
                        opts => { opts.Bind<QueueJobEventConsumer>(); });
                });

                services.AddMassTransitHostedService(true);
            });
        }
    }
}