using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Database;
using CIPlatform.Data.Events;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Cache;
using CIPlatformManager.Configuration;
using CIPlatformManager.Repositories;
using CIPlatformManager.Services;
using CIPlatformManager.SignalR;
using CIPlatformManager.SignalR.Hubs;
using CIPlatformWorker.Hubs;
using CIPlatformManager.Extensions;
using CIPlatformManager.Repositories.Jobs;
using CIPlatformManager.Repositories.Workers;
using CIPlatformManager.Services.Auth;
using CIPlatformManager.Services.Jobs;
using CIPlatformManager.Services.Logs;
using CIPlatformManager.Services.Runs;
using CIPlatformManager.Services.Steps;
using CIPlatformManager.Services.Tasks;
using CIPlatformManager.Services.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CIPlatformManager
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
                        "v1", new OpenApiInfo {Title = "CIPlatformManager", Version = "v1"});
                })
                .AddControllers().Services
                .AddDbContext<CIPlatformDbContext>(options =>
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
                .Configure<SignalRConfiguration>(Configuration.GetSection("SignalRConfiguration"));

            // Services with singleton lifetime
            services
                .AddSingleton<IRedisConnectionCache, RedisCache>()
                .AddSingleton<IRedisJobQueueCache, RedisJobQueueCache>()
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
                .AddScoped<IStepService<StepResultEvent>, StepService>()
                .AddScoped<IJobService, JobService>()
                .AddScoped<IJobQueueService, JobQueueService>()
                .AddScoped<IJobQueueRepository, JobQueueRepository>();

            services.AddHostedService<QueueService>();
            services.AddHostedService<WorkerCleanupService>();
            services.AddHostedService<Manager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CIPlatformManager v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<WorkerHub>("/worker");
                endpoints.MapHub<WebApiHub>("/webapi");
            });
        }
    }
}