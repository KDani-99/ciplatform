using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Database;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.Cache;
using CodeManagerWebApi.Configuration;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Extensions;
using CodeManagerWebApi.Hubs;
using CodeManagerWebApi.Repositories;
using CodeManagerWebApi.Services;
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
                .AddCors()
                .AddFluentValidation()
                .Configure<JwtConfiguration>(_configuration.GetSection("JwtConfiguration"))
                .Configure<RedisConfiguration>(_configuration.GetSection("RedisConfiguration"))
                .Configure<RedisConfiguration>(_configuration.GetSection("YmlConfiguration"))
                .Configure<IConfiguration>(_configuration)
                .AddDbContext<CodeManagerDbContext, Database.CodeManagerDbContext>(options =>
                    options.UseNpgsql(_configuration.GetValue<string>("ConnectionString")))
                .AddSingleton<ICredentialManagerService, CredentialManagerService>()
                .AddSingleton(new JsonSerializerOptions
                {
                    Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                })
                .AddScoped(_ => new DeserializerBuilder()
                                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                                .Build())
                .AddScoped<ITokenService<JwtSecurityToken>, TokenService>()
                .AddSingleton<ITokenCache, RedisTokenCache>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<ITeamRepository, TeamRepository>()
                .AddScoped<ITeamService, TeamService>()
                .AddScoped<IProjectRepository, ProjectRepository>()
                .AddScoped<IProjectService, ProjectService>()
                .AddScoped<ITokenRepository, TokenRepository>()
                .AddScoped<IRunRepository, RunRepository>()
                .AddScoped<IRunService, RunService>()
                .AddScoped<IFileProcessorService<RunConfiguration>, YmlFileProcessorService>()
                .AddAntiforgery()
                .AddRabbitMq(_configuration)
                .AddControllers().Services
                .AddSignalR().Services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo {Title = "CodeManagerWebApi", Version = "v1"});
                })
                .AddHealthChecks();

            services
                .AddTransient<IValidator<CreateProjectDto>, CreateProjectDtoValidator>()
                .AddTransient<IValidator<TeamDto>, TeamDtoValidator>()
                .AddTransient<IValidator<LoginDto>, LoginDtoValidator>()
                .AddTransient<IValidator<CreateUserDto>, CreateUserDtoValidator>()
                .AddTransient<IValidator<UpdateUserDto>, UpdateUserDtoValidator>();

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 2;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            services.AddAuthentication("JwtAuthToken")
                    .AddJwtBearer(options =>
                    {
                        options.MapInboundClaims = false;

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    path.StartsWithSegments("/runs"))
                                    context.Token = accessToken;

                                return Task.CompletedTask;
                            }
                        };
                    })
                    .AddScheme<JwtAuthenticationTokenSchemeOptions, JwtAuthenticationHandler>("JwtAuthToken", null);
        }

        public void Configure(IApplicationBuilder app,
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