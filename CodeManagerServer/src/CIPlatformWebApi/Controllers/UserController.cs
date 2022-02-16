using System;
using System.Net;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CIPlatformWebApi.Controllers
{
    [ApiController]
    [Route("/api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] long id)
        {
            var user = HttpContext.Items["user"] as User;

            var userDto = await _userService.GetUserAsync(id, user);

            return Ok(userDto);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUserAsync()
        {
            var user = HttpContext.Items["user"] as User;
            var userDto = await _userService.GetUserAsync(user.Id, user);

            return Ok(userDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("{id:long}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] long id, [FromBody] UpdateUserDto updateUserDto)
        {
            var user = HttpContext.Items["user"] as User;
            await _userService.UpdateUserAsync(id, updateUserDto, user);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] long id)
        {
            var user = HttpContext.Items["user"] as User;
            await _userService.DeleteUserAsync(id, user);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("/api/users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var user = HttpContext.Items["user"] as User;
            var usersDto = await _userService.GetUsersAsync(user);

            return Ok(usersDto);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CreateUserDto createUserDto)
        {
            await _userService.CreateUser(createUserDto);
            _logger.LogInformation($"User `{createUserDto.Username}` has been created @ {DateTime.Now}");

            return StatusCode((int) HttpStatusCode.Created);
        }
    }
}