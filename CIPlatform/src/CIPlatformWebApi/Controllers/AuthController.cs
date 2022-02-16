using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CIPlatformWebApi.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IUserService _userService;

        public AuthController(IUserService userService,
                              ITokenService<JwtSecurityToken> tokenService,
                              ILogger<AuthController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            var authTokens = await _userService.LoginAsync(loginDto, HttpContext);
            _logger.LogInformation($"User `{loginDto.Username}` has logged in @ {DateTime.Now}");

            return Ok(authTokens);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[1];

                var claimsPrincipal = await _tokenService.VerifyRefreshTokenAsync(token);
                var username = claimsPrincipal.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;

                await _tokenService.InvalidateAccessTokenAsync(username);
                await _tokenService.InvalidRefreshTokenAsync(username);
                _logger.LogInformation($"User `{username}` has logged out @ {DateTime.Now}");

                return NoContent();
            }
            catch (BadHttpRequestException)
            {
                throw;
            }
            catch
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[1];

                var claimsPrincipal = await _tokenService.VerifyRefreshTokenAsync(token);
                var username = claimsPrincipal.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
                var authTokens = await _userService.GenerateAuthTokensAsync(username);
                _logger.LogInformation($"Refreshed tokens for `{username}` @ {DateTime.Now}");

                return Ok(authTokens);
            }
            catch (BadHttpRequestException)
            {
                throw;
            }
            catch
            {
                return Unauthorized();
            }
        }
    }
}