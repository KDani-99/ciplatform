using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeManagerWebApi.Controllers
{
    [ApiController]
    [Route("/api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IRequestClient<QueueRunCommand> _requestClient;
        
        public UserController(IUserService userService, ILogger<UserController> logger, IRequestClient<QueueRunCommand> requestClient)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestClient = requestClient;
        }

        [HttpPost, Route("test")]
        public async Task<IActionResult> QueueRun()
        {
            string content = await new StreamReader(Request.Body).ReadToEndAsync();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            
            var configuration = deserializer.Deserialize<RunConfiguration>(content);
            
            var response = await _requestClient.GetResponse<SuccessfulQueueRunCommandResponse, FailedQueueRunCommandResponse>(new QueueRunCommand {RunConfiguration = configuration, Repository = "https://github.com/KDani-99/doc-assistant"});

            return Ok();
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            await _userService.CreateUser(userDto);
            _logger.LogInformation($"User `{userDto.Username}` created @ {DateTime.Now}");

            return StatusCode((int)HttpStatusCode.Created);
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