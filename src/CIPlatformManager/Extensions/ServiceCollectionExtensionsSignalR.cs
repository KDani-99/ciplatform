using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CIPlatformManager.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRClient(this IServiceCollection serviceCollection,
                                                          IConfiguration configuration)
        {
            var wsConfiguration = configuration.GetSection("SignalRConfiguration").Get<SignalRConfiguration>();
            var connection = new HubConnectionBuilder()
                             .WithUrl($"{wsConfiguration.Host}/{wsConfiguration.Hub}",
                                      options => options.AccessTokenProvider = () => Task.FromResult(""))
                             .Build();

            connection.StartAsync().Wait(); // TODO:

            return serviceCollection.AddSingleton(connection);
        }
    }
}