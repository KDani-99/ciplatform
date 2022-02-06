using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;

namespace CodeManagerWebApi.Services
{
    public class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationTokenSchemeOptions>
    {
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<JwtAuthenticationHandler> _logger;
        
        public JwtAuthenticationHandler(ITokenService<JwtSecurityToken> tokenService, IUserRepository userRepository, IOptionsMonitor<JwtAuthenticationTokenSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
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

                var user = await _userRepository.GetByUsernameAsync(username) ?? throw new InvalidCredentialsException();

                Context.Items.Add("user", user);

                var x = user.Roles.Select(role => new Claim(ClaimTypes.Role, role.GetDisplayName()));

                claimsPrincipal.Identities.First().AddClaims(x);
                
                var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception)
            {
                _logger.LogInformation($"Invalid authentication attempt.");
                return AuthenticateResult.Fail("Invalid token.");
            }
        }
    }
}