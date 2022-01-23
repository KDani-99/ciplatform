using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Database;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Configuration;
using CodeManagerAgentManager.Extensions;
using CodeManagerAgentManager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeManagerAgentManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(new JsonSerializerOptions
                        {
                            Converters =
                            {
                                new JsonStringEnumConverter()
                            }
                        })
                        .Configure<TokenServiceConfiguration>(hostContext.Configuration.GetSection("TokenConfiguration"))
                        .AddDbContext<CodeManagerDbContext>(options => options.UseNpgsql(hostContext.Configuration.GetValue<string>("ConnectionString")))
                        .AddScoped<IRunRepository, RunRepository>()
                        .AddScoped<IRunService<QueueRunCommand>, RunService>()
                        .AddScoped<ITokenService<JwtSecurityToken>, TokenService>()
                        .AddScoped<IRunRepository, RunRepository>()
                        .AddRabbitMq(hostContext.Configuration);
                });
    }
}