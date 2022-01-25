using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeManager.Data.Commands;
using CodeManager.Data.Database;
using CodeManager.Data.Repositories;
using CodeManagerAgent.Hubs;
using CodeManagerAgentManager.Configuration;
using CodeManagerAgentManager.Extensions;
using CodeManagerAgentManager.Services;
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "CodeManagerAgentManager", Version = "v1"});
            });
            services.AddSingleton(new JsonSerializerOptions
                {
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                })
                .Configure<TokenServiceConfiguration>(
                    Configuration.GetSection("TokenConfiguration"))
                .Configure<LogStreamServiceConfiguration>(
                    Configuration.GetSection("LogStreamServiceConfiguration"))
                .AddDbContext<CodeManagerDbContext>(options =>
                    options.UseNpgsql(Configuration.GetValue<string>("ConnectionString")))
                .AddScoped<IRunRepository, RunRepository>()
                .AddScoped<IRunService<QueueRunCommand>, RunService>()
                .AddScoped<ITokenService<JwtSecurityToken>, TokenService>()
                .AddScoped<IRunRepository, RunRepository>()
                .AddScoped<ILogStreamService, LogStreamService>()
                .AddSignalR().Services
                .AddRabbitMq(Configuration);
            //.AddSignalRClient(Configuration);
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
            });
        }
    }
}