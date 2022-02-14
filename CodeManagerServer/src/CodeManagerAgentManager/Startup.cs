using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeManager.Data.Configuration;
using CodeManager.Data.Database;
using CodeManager.Data.Events;
using CodeManager.Data.Repositories;
using CodeManagerAgent.Hubs;
using CodeManagerAgentManager.Cache;
using CodeManagerAgentManager.Configuration;
using CodeManagerAgentManager.Extensions;
using CodeManagerAgentManager.Repositories;
using CodeManagerAgentManager.Services;
using CodeManagerAgentManager.WebSocket;
using CodeManagerAgentManager.WebSocket.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CodeManagerAgentManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Required services
            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(
                        "v1", new OpenApiInfo {Title = "CodeManagerAgentManager", Version = "v1"});
                })
                .AddControllers().Services
                .AddDbContext<CodeManagerDbContext>(options =>
                                                        options.UseNpgsql(
                                                            Configuration.GetValue<string>("ConnectionString"),
                                                            builder => builder.UseQuerySplittingBehavior(
                                                                QuerySplittingBehavior.SplitQuery)))
                .AddSignalR().Services
                .AddRabbitMq(Configuration);

            // Configuration
            services
                .Configure<TokenServiceConfiguration>(
                    Configuration.GetSection("TokenConfiguration"))
                .Configure<LogStreamServiceConfiguration>(
                    Configuration.GetSection("LogStreamServiceConfiguration"))
                .Configure<RedisConfiguration>(Configuration.GetSection("RedisConfiguration"))
                .Configure<WebSocketConfiguration>(Configuration.GetSection("WebSocketConfiguration"));

            // Services with singleton lifetime
            services
                .AddSingleton<IConnectionCache, RedisConnectionCache>()
                .AddSingleton<IManagerClient, ManagerClient>()
                .AddSingleton(new JsonSerializerOptions
                {
                    Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                });

            // Services with scoped lifetime
            services
                .AddScoped<IWorkerConnectionRepository, WorkerConnectionRepository>()
                .AddScoped<IRunRepository, RunRepository>()
                .AddScoped<IProjectRepository, ProjectRepository>()
                .AddScoped<IRunService, RunService>()
                .AddScoped<ITokenService<JwtSecurityToken>, TokenService>()
                .AddScoped<IRunRepository, RunRepository>()
                .AddScoped<ILogStreamService, LogStreamService>()
                .AddScoped<IWorkerConnectionService, WorkerConnectionService>()
                .AddScoped<IStepService<StepResultEvent>, StepService>();

            services.AddHostedService<Manager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeManagerAgentManager v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<AgentHub>("/agent");
                endpoints.MapHub<WebApiHub>("/webapi");
            });
        }
    }
}