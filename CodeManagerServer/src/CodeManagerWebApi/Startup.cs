using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeManagerWebApi.Database;
using CodeManagerWebApi.Entities.Configuration;
using CodeManagerWebApi.Repositories;
using CodeManagerWebApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*var databaseConfiguration = _configuration.GetSection("DatabaseConfiguration").Get<DatabaseConfiguration>();
            var mongoClient = new MongoClient();*/

            services
                .Configure<JwtConfiguration>(_configuration.GetSection("JwtConfiguration"))
                .Configure<UserConfiguration>(_configuration.GetSection("UserConfiguration"))
                //.AddSingleton(mongoClient.GetDatabase(databaseConfiguration.Database))
                .AddDbContext<CodeManagerDbContext>(options => options.UseNpgsql())
                .AddSingleton<ICredentialManagerService, CredentialManagerService>()
                .AddSingleton<ITokenService<JwtSecurityToken>, TokenService>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<ITeamRepository, TeamRepository>()
                .AddSingleton<ITeamService, TeamService>()
                .AddAntiforgery()
                
                .AddControllers().Services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo {Title = "CodeManagerWebApi", Version = "v1"});
                })
                .AddHealthChecks();
            
            services.AddAuthentication("JwtAuthToken")
                .AddJwtBearer(opts => opts.MapInboundClaims = false)
                .AddScheme<JwtAuthenticationTokenSchemeOptions, JwtAuthenticationHandler>("JwtAuthToken",null);
            
            // Configure authentication (TODO: extension class)
           /* services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var jwtConfiguration = _configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>();
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfiguration.Secret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        RequireExpirationTime = true
                    };
                    options.
                });*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IUserRepository userRepository,
            ICredentialManagerService credentialManagerService,
            ILogger<BootstrapService> logger)
        {
            Bootstrap(userRepository, credentialManagerService, logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeManagerWebApi v1"));
            }

            app
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); })
                .UseHealthChecks("/api/health");
        }

        private void Bootstrap(
            IUserRepository userRepository,
            ICredentialManagerService credentialManagerService,
            ILogger<BootstrapService> logger)
        {
            var accountConfiguration = _configuration.GetSection("ServiceAccountConfiguration").Get<ServiceAccountConfiguration>();

            new BootstrapService(userRepository, credentialManagerService, logger).Invoke(accountConfiguration).Wait();
        }
    }
}