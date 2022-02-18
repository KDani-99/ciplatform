using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CIPlatform.Data.Repositories;
using CIPlatformWebApi.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;

namespace CIPlatformWebApi.Services.Auth
{
    public class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationTokenSchemeOptions>
    {
        private readonly ILogger<JwtAuthenticationHandler> _logger;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IUserRepository _userRepository;

        public JwtAuthenticationHandler(ITokenService<JwtSecurityToken> tokenService,
                                        IUserRepository userRepository,
                                        IOptionsMonitor<JwtAuthenticationTokenSchemeOptions> options,
                                        ILoggerFactory logger,
                                        UrlEncoder encoder,
                                        ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger.CreateLogger<JwtAuthenticationHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Invalid token.");
            }

            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[1];

                var claimsPrincipal = await _tokenService.VerifyAccessTokenAsync(token);
                var username = claimsPrincipal.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;

                var user = await _userRepository.GetByUsernameAsync(username) ??
                    throw new InvalidCredentialsException();

                Context.Items.Add("user", user);

                var claims = user.Roles.Select(role => new Claim(ClaimTypes.Role, role.GetDisplayName()));

                claimsPrincipal.Identities.First().AddClaims(claims);

                var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (SecurityTokenExpiredException)
            {
                Context.Response.Headers.Add("Token-Expired", "1");
                _logger.LogInformation("Invalid authentication attempt with expired token.");
                return AuthenticateResult.Fail("Token has expired.");
            }
            catch (Exception)
            {
                _logger.LogInformation("Invalid authentication attempt.");
                return AuthenticateResult.Fail("Invalid token.");
            }
        }

        protected override async Task InitializeEventsAsync()
        {
            Options.Events = new JwtBearerEvents
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
        }

        private Task HandleWsAuthenticationAsync(MessageReceivedContext context)
        {
            /*   try
               {
                   var accessToken = context.Request.Query["access_token"];
   
                   var claimsPrincipal = await _tokenService.VerifyAccessTokenAsync(accessToken);
                   var username = claimsPrincipal.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
   
                   var user = await _userRepository.GetByUsernameAsync(username) ?? throw new InvalidCredentialsException();
   
                   Context.Items.Add("user", user);
   
                   var claims = user.Roles.Select(role => new Claim(ClaimTypes.Role, role.GetDisplayName()));
   
                   claimsPrincipal.Identities.First().AddClaims(claims);
                   
               }
               catch (Exception)
               {
                   _logger.LogInformation($"Invalid authentication attempt.");
                   return AuthenticateResult.Fail("Invalid token.");
               }*/

            var accessToken = context.Request.Query["access_token"];

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/runs"))
                // Read the token out of the query string
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    }
}