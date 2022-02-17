using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Database;
using CIPlatform.Data.Repositories;
using CIPlatformWebApi.Cache;
using CIPlatformWebApi.Configuration;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.Repositories;
using CIPlatformWebApi.Services;
using CIPlatformWebApi.WebSocket.Hubs;
using CIPlatformWebApi.Extensions;
using CIPlatformWebApi.Extensions.Services;
using CIPlatformWebApi.Strategies;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CIPlatformWebApi
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
            // Required services
            services
                .AddCors()
                .AddFluentValidation()
                .AddDbContext<CIPlatformDbContext, Database.CiPlatformDbContext>(options =>
                    options.UseNpgsql(_configuration.GetValue<string>("ConnectionString"),
                                      builder => builder.UseQuerySplittingBehavior(
                                          QuerySplittingBehavior.SplitQuery)))
                .AddAntiforgery()
                .AddRabbitMq(_configuration)
                .AddControllers().Services
                .AddSignalR().Services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CIPlatformWebApi", Version = "v1" });
                })
                .AddAuthentication("JwtAuthToken")
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                })
                .AddScheme<JwtAuthenticationTokenSchemeOptions, JwtAuthenticationHandler>("JwtAuthToken", (options) =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/runs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                }).Services
                .AddHealthChecks();

            // Configuration
            services
                .Configure<JwtConfiguration>(_configuration.GetSection("JwtConfiguration"))
                .Configure<RedisConfiguration>(_configuration.GetSection("RedisConfiguration"))
                .Configure<YmlConfiguration>(_configuration.GetSection("YmlConfiguration"))
                .Configure<IConfiguration>(_configuration)
                .Configure<FormOptions>(options =>
                {
                    options.ValueCountLimit = 2;
                    options.ValueLengthLimit = int.MaxValue;
                    options.MultipartBodyLengthLimit = long.MaxValue;
                });

            // Services with singleton lifetime
            services
                .AddSingleton(new JsonSerializerOptions
                {
                    Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                })
                .AddSingleton<ICredentialManagerService, CredentialManagerService>()
                .AddSingleton<ITokenCache, RedisTokenCache>();

            // Services with scoped lifetime
            services
                .AddScoped(_ => new DeserializerBuilder()
                                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                                .Build())
                .AddScoped<ITokenService<JwtSecurityToken>, TokenService>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<ITeamRepository, TeamRepository>()
                .AddScoped<ITeamService, TeamService>()
                .AddScoped<IProjectRepository, ProjectRepository>()
                .AddScoped<IProjectService, ProjectService>()
                .AddScoped<ITokenRepository, TokenRepository>()
                .AddScoped<IRunRepository, RunRepository>()
                .AddScoped<IRunService, RunService>()
                .AddScoped<IResultChannelConnectionHandlerFactory, ResultChannelConnectionHandlerFactory>()
                .AddScoped<IFileProcessorService<RunConfiguration>, YmlFileProcessorService>();

            // Services with transient lifetime
            services
                .AddTransient<IValidator<CreateProjectDto>, CreateProjectDtoValidator>()
                .AddTransient<IValidator<TeamDto>, TeamDtoValidator>()
                .AddTransient<IValidator<LoginDto>, LoginDtoValidator>()
                .AddTransient<IValidator<CreateUserDto>, CreateUserDtoValidator>()
                .AddTransient<IValidator<UpdateUserDto>, UpdateUserDtoValidator>();
        }

        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CIPlatformWebApi v1"));
            }

            app
                .UseExceptionHandler("/error")
                .UseHttpsRedirection()
                .UseRouting()
                .UseCors(x => x
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .WithExposedHeaders("Token-Expired")
                              .SetIsOriginAllowed(origin => true) // any
                              //.WithOrigins("https://localhost:4000"));
                              .AllowCredentials())
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHub<RunsHub>("/runs");
                    endpoints.MapHub<ManagerHub>("/manager");
                })
                .UseHealthChecks("/api/health");
        }
    }
}