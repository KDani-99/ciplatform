using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeManagerAgentManager.Commands;
using CodeManagerWebApi.Database;
using CodeManagerWebApi.Entities.Configuration;
using CodeManagerWebApi.Repositories;
using CodeManagerWebApi.Services;
using CodeManagerWebApi.Utils.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace CodeManagerWebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<JwtConfiguration>(_configuration.GetSection("JwtConfiguration"))
                .Configure<UserConfiguration>(_configuration.GetSection("UserConfiguration"))
                .Configure<IConfiguration>(_configuration)
                .AddDbContext<CodeManagerDbContext>(options => options.UseNpgsql(_configuration.GetValue<string>("ConnectionString")))
                .AddSingleton<ICredentialManagerService, CredentialManagerService>()
                .AddSingleton<ITokenService<JwtSecurityToken>, TokenService>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<ITeamRepository, TeamRepository>()
                .AddScoped<ITeamService, TeamService>()
                .AddScoped<IPlanRepository, PlanRepository>()
                .AddScoped<IPlanService, PlanService>()
                .AddAntiforgery()
                .AddMassTransit(options =>
                {
                    var bus = Bus.Factory.CreateUsingRabbitMq(config =>
                    {
                        config.Host("localhost", "/",
                            h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });
                    });
                    options.AddRequestClient<StartJobCommand>();
                    
                    bus.Start();
                })
                .AddControllers().Services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo {Title = "CodeManagerWebApi", Version = "v1"});
                })
                .AddHealthChecks();
            
            services.AddAuthentication("JwtAuthToken")
                .AddJwtBearer(opts => opts.MapInboundClaims = false)
                .AddScheme<JwtAuthenticationTokenSchemeOptions, JwtAuthenticationHandler>("JwtAuthToken",null);
        }
        
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeManagerWebApi v1"));
            }

            app
                .UseExceptionHandler("/error")
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); })
                .UseHealthChecks("/api/health");
        }
    }
}