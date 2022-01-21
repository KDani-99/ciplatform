using System;
using CodeManager.Data.Configuration;
using CodeManagerAgentManager.Configuration;
using CodeManagerAgentManager.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeManagerAgentManager.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddMassTransit(busConfigurator =>
            {
                var massTransitConfiguration = configuration.GetSection("MassTransitConfiguration").Get<MassTransitConfiguration>();
                
                // TODO: make consumers temporary

                busConfigurator.AddConsumer<StepLogEventConsumer>();
                busConfigurator.AddConsumer<StepResultEventConsumer>();
                busConfigurator.AddConsumer<RequestJobCommandConsumer>();
                
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(massTransitConfiguration.Host, massTransitConfiguration.VirtualHost,
                        h =>
                        {
                            h.Username(massTransitConfiguration.Username);
                            h.Password(massTransitConfiguration.Password);
                        });
                    
                    cfg.ReceiveEndpoint(massTransitConfiguration.Queues["StepLogEventQueue"],
                        opts => { opts.Bind<StepLogEventConsumer>();
                            opts.AutoDelete = true;
                        });
                    cfg.ReceiveEndpoint(massTransitConfiguration.Queues["StepResultEventQueue"],
                        opts => { opts.Bind<StepResultEventConsumer>(); opts.AutoDelete = true; });
                    cfg.ReceiveEndpoint(massTransitConfiguration.Queues["RequestJobCommandQueue"],
                        opts => { opts.Bind<RequestJobCommandConsumer>(); opts.AutoDelete = true; });
                });

                services.AddMassTransitHostedService(true);
            });
        }
    }
}