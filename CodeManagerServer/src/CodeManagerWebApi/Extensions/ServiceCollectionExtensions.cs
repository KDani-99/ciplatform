using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
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
                var massTransitConfiguration = configuration.GetSection("MassTransitConfiguration").Get<MassTransitConfiguration>();

                busConfigurator.AddRequestClient<QueueRunCommand>();
                
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(massTransitConfiguration.Host, massTransitConfiguration.VirtualHost,
                        h =>
                        {
                            h.Username(massTransitConfiguration.Username);
                            h.Password(massTransitConfiguration.Password);
                        });
                });
                
                services.AddMassTransitHostedService(true);
            });
        }
    }
}