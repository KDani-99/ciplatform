using System;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeManagerAgent.Extensions
{
    public static class ServiceCollectionExtensionsSignalR
    {
        public static IServiceCollection AddSignalRClient(this IServiceCollection serviceCollection,
                                                          IConfiguration configuration)
        {
            var wsConfiguration = configuration.GetSection("WebSocketConfiguration").Get<WebSocketConfiguration>();
            var connection = new HubConnectionBuilder()
                             .WithUrl($"{wsConfiguration.Host}/{wsConfiguration.Hub}",
                                      options => options.AccessTokenProvider = () => Task.FromResult(""))
                             .Build();

            connection.Closed += async error =>
            {
                Console.WriteLine("Closed");
                await connection.StartAsync();
            };
            // TODO: 
            connection.StartAsync().Wait();

            Console.WriteLine("Connected");

            return serviceCollection.AddSingleton(connection);
        }
    }
}