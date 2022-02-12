using CodeManager.Data.Configuration;
using CodeManagerWebApi.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeManagerWebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddMassTransit(busConfigurator =>
            {
                var massTransitConfiguration =
                    configuration.GetSection("MassTransitConfiguration").Get<MassTransitConfiguration>();

                //busConfigurator.AddRequestClient<QueueRunCommand>();
                busConfigurator.AddConsumer<ProcessedStepResultEventConsumer>();

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
                });

                services.AddMassTransitHostedService(true);
            });
        }
    }
}