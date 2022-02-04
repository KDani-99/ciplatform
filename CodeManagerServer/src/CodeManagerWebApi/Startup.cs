using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Core.Services;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.Database;
using CodeManagerWebApi.Configuration;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Extensions;
using CodeManagerWebApi.Hubs;
using CodeManagerWebApi.Services;
using CodeManagerWebApi.Utils.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
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
                .AddCors()
                .AddFluentValidation()
                .Configure<JwtConfiguration>(_configuration.GetSection("JwtConfiguration"))
                .Configure<UserConfiguration>(_configuration.GetSection("UserConfiguration"))
                .Configure<IConfiguration>(_configuration)
                .AddDbContext<CodeManager.Data.Database.CodeManagerDbContext, CodeManagerDbContext>(options =>
                    options.UseNpgsql(_configuration.GetValue<string>("ConnectionString")))
                .AddSingleton<ICredentialManagerService, CredentialManagerService>()
                .AddSingleton<ITokenService<JwtSecurityToken>, TokenService>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<ITeamRepository, TeamRepository>()
                .AddScoped<ITeamService, TeamService>()
                .AddScoped<IProjectRepository, ProjectRepository>()
                .AddScoped<IProjectService, ProjectService>()
                .AddScoped<IVariableRepository, VariableRepository>()
                .AddScoped<IVariableService, VariableService>()
                .AddScoped<IEncryptionService, EncryptionService>()
                .AddScoped<IPlanRepository, PlanRepository>()
                .AddScoped<IPlanService, PlanService>()
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
                .AddTransient<IValidator<VariableDto>, VariableDtoValidator>();
            
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
                .UseCors(x => x
                    .AllowAnyMethod()
                    .AllowAnyHeader()
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