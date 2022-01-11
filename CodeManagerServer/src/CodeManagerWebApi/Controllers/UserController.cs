using System;
using System.Threading.Tasks;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace CodeManagerWebApi.Controllers
{
    [ApiController]
    [Route("/api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            await _userService.CreateUser(userDto);
            _logger.LogInformation($"User `{userDto.Username}` created");

            return StatusCode(201);
        }
        
        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var authTokens = await _userService.LoginAsync(loginDto, HttpContext);
            _logger.LogInformation($"User `{loginDto.Username}` logged in @ {DateTime.Now}");
                
            return Ok(authTokens);
        }
    }
}