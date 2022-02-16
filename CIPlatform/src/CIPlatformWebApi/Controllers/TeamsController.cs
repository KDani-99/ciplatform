using System;
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
            var user = HttpContext.Items["user"] as User;
            var teams = await _teamService.GetTeamsAsync(user);

            return Ok(teams);
        }

        [HttpGet]
        [Route("{id:long}", Name = nameof(GetTeamAsync))]
        public async Task<IActionResult> GetTeamAsync([FromRoute] long id)
        {
            var user = HttpContext.Items["user"] as User;
            var team = await _teamService.GetTeamAsync(id, user);

            return Ok(team);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamAsync([FromBody] TeamDto teamDto)
        {
            var user = HttpContext.Items["user"] as User;
            var result = await _teamService.CreateTeamAsync(teamDto, user);
            _logger.LogInformation($"Team `{teamDto.Name}` has been created @ {DateTime.Now}");

            return CreatedAtRoute(nameof(GetTeamAsync), new { result.Id }, result);
        }

        [HttpPut]
        [Route("{id:long}")]
        public async Task<IActionResult> UpdateTeamAsync([FromRoute] long id, [FromBody] TeamDto teamDto)
        {
            teamDto.Id = id;

            var user = HttpContext.Items["user"] as User;
            await _teamService.UpdateTeamAsync(teamDto, user);
            _logger.LogInformation($"Team `{teamDto.Name}` has been updated @ {DateTime.Now}");

            return Ok();
        }

        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> DeleteTeamAsync([FromRoute] long id)
        {
            var user = HttpContext.Items["user"] as User;
            await _teamService.DeleteTeamAsync(id, user);
            _logger.LogInformation($"Team with id `{id}` has been deleted @ {DateTime.Now}");

            return NoContent();
        }

        [HttpPost]
        [Route("{id:long}/join")]
        public async Task<IActionResult> Join([FromRoute] long id)
        {
            var user = HttpContext.Items["user"] as User;
            await _teamService.JoinAsync(id, user);
            _logger.LogInformation($"User `{user.Name}` has joined team id `{id}` @ {DateTime.Now}");

            return NoContent();
        }

        [HttpPost]
        [Route("{id:long}/members/kick")]
        public async Task<IActionResult> Kick([FromRoute] long id, [FromBody] KickMemberDto kickMemberDto)
        {
            var user = HttpContext.Items["user"] as User;
            await _teamService.KickMemberAsync(id, kickMemberDto.MemberId, user);
            _logger.LogInformation(
                $"User id `{kickMemberDto.MemberId}` has been kicked from team id `{id}` @ {DateTime.Now}");

            return NoContent();
        }

        [HttpPost]
        [Route("{id:long}/members/role")]
        public async Task<IActionResult> UpdateRole([FromRoute] long id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            var user = HttpContext.Items["user"] as User;
            await _teamService.UpdateRoleAsync(id, updateRoleDto, user);
            _logger.LogInformation(
                $"User id `{updateRoleDto.UserId}` role has been updated to {updateRoleDto.Role} @ {DateTime.Now}");

            return NoContent();
        }

        [HttpPost]
        [Route("{id:long}/members/add")]
        public async Task<IActionResult> AddMember([FromRoute] long id, [FromBody] AddMemberDto addMemberDto)
        {
            var user = HttpContext.Items["user"] as User;
            await _teamService.AddMemberAsync(id, addMemberDto, user);

            return NoContent();
        }
    }
}