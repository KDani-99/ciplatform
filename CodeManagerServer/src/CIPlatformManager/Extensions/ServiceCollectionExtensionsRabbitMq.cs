using CIPlatform.Data.Configuration;
using CIPlatformManager.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CIPlatformManager.Extensions
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