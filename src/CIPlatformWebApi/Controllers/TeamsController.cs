using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Team;
using CIPlatformWebApi.Extensions.Utils;
using CIPlatformWebApi.Services;
using CIPlatformWebApi.Services.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CIPlatformWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    [Route("/api/teams")]
    [Produces("application/json")]
    public class TeamsController : ControllerBase
    {
        private readonly ILogger<TeamsController> _logger;
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService, ILogger<TeamsController> logger)
        {
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetTeamsAsync()
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                var teams = await _teamService.GetTeamsAsync(user);

                return Ok(teams);
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpGet]
        [Route("{id:long}", Name = nameof(GetTeamAsync))]
        public async Task<IActionResult> GetTeamAsync([FromRoute] long id)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                var team = await _teamService.GetTeamAsync(id, user);

                return Ok(team);
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamAsync([FromBody] TeamDto teamDto)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                var result = await _teamService.CreateTeamAsync(teamDto, user);
                _logger.LogInformation($"Team `{teamDto.Name}` has been created @ {DateTime.Now}");

                return CreatedAtRoute(nameof(GetTeamAsync), new { result.Id }, result);
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpPut]
        [Route("{id:long}")]
        public async Task<IActionResult> UpdateTeamAsync([FromRoute] long id, [FromBody] TeamDto teamDto)
        {
            try
            {
                teamDto.Id = id;

                var user = HttpContext.Items["user"] as UserEntity;
                await _teamService.UpdateTeamAsync(teamDto, user);
                _logger.LogInformation($"Team `{teamDto.Name}` has been updated @ {DateTime.Now}");

                return Ok();
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> DeleteTeamAsync([FromRoute] long id)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                await _teamService.DeleteTeamAsync(id, user);
                _logger.LogInformation($"Team with id `{id}` has been deleted @ {DateTime.Now}");

                return NoContent();
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpPost]
        [Route("{id:long}/join")]
        public async Task<IActionResult> JoinAsync([FromRoute] long id)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                await _teamService.JoinAsync(id, user);
                _logger.LogInformation($"User `{user.Name}` has joined team id `{id}` @ {DateTime.Now}");

                return NoContent();
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpPost]
        [Route("{id:long}/members/kick")]
        public async Task<IActionResult> KickAsync([FromRoute] long id, [FromBody] KickMemberDto kickMemberDto)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                await _teamService.KickMemberAsync(id, kickMemberDto.MemberId, user);
                _logger.LogInformation(
                    $"User id `{kickMemberDto.MemberId}` has been kicked from team id `{id}` @ {DateTime.Now}");

                return NoContent();
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpPost]
        [Route("{id:long}/members/role")]
        public async Task<IActionResult> UpdateRoleAsync([FromRoute] long id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                await _teamService.UpdateRoleAsync(id, updateRoleDto, user);
                _logger.LogInformation(
                    $"User id `{updateRoleDto.UserId}` role has been updated to {updateRoleDto.Role} @ {DateTime.Now}");

                return NoContent();
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpPost]
        [Route("{id:long}/members/add")]
        public async Task<IActionResult> AddMemberAsync([FromRoute] long id, [FromBody] AddMemberDto addMemberDto)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                await _teamService.AddMemberAsync(id, addMemberDto, user);

                return NoContent();
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }
    }
}