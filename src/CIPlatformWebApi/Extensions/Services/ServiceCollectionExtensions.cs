using CIPlatform.Data.Configuration;
using CIPlatformWebApi.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CIPlatformWebApi.Extensions.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddMassTransit(busConfigurator =>
            {
                var massTransitConfiguration =
                    configuration.GetSection("MassTransitConfiguration").Get<MassTransitConfiguration>();

                busConfigurator.AddConsumer<ProcessedStepResultEventConsumer>();
                busConfigurator.AddConsumer<ProcessedJobResultEventConsumer>();
                busConfigurator.AddConsumer<ProcessedRunResultEventConsumer>();

                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(massTransitConfiguration.Host, massTransitConfiguration.VirtualHost,
                        h =>
                        {
                            h.Username(massTransitConfiguration.Username);
                            h.Password(massTransitConfiguration.Password);
                        });
                    cfg.ReceiveEndpoint(massTransitConfiguration.Queues["ProcessedStepResultEventQueue"],
                        opts =>
                        {
                            opts.AutoDelete = true;
                            opts.ConfigureConsumer<ProcessedStepResultEventConsumer>(context);
                        });
                    cfg.ReceiveEndpoint(massTransitConfiguration.Queues["ProcessedJobResultEventQueue"],
                        opts =>
                        {
                            opts.AutoDelete = true;
                            opts.ConfigureConsumer<ProcessedJobResultEventConsumer>(context);
                        });
                    cfg.ReceiveEndpoint(massTransitConfiguration.Queues["ProcessedRunResultEventQueue"],
                        opts =>
                        {
                            opts.AutoDelete = true;
                            opts.ConfigureConsumer<ProcessedRunResultEventConsumer>(context);
                        });
                });

                services.AddMassTransitHostedService(true);
            });
        }
    }
}