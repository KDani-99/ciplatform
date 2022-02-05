using System;
using System.Net;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;

namespace CodeManagerWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    [Route("/api/teams")]
    [Produces("application/json")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(ITeamService teamService, ILogger<TeamsController> logger)
        {
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = HttpContext.Items["user"] as User;
            var teams = await _teamService.GetTeamsAsync(user);
            
            return Ok(teams);
        }
        
        [HttpGet, Route("{id:long}", Name = nameof(Get))]
        public async Task<IActionResult> Get([FromRoute] long id)
        {
            var user = HttpContext.Items["user"] as User;
            var team = await _teamService.GetTeamAsync(id, user);
            
            return Ok(team);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TeamDto teamDto)
        {
            var user = HttpContext.Items["user"] as User;
            var result = await _teamService.CreateTeamAsync(teamDto, user);
            _logger.LogInformation($"Team `{teamDto.Name}` created @ {DateTime.Now}");

            return CreatedAtRoute(nameof(Get), new {result.Id}, result);
        }
        
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] TeamDto teamDto)
        {
            var user = HttpContext.Items["user"] as User;
            await _teamService.UpdateTeamAsync(teamDto, user);
            _logger.LogInformation($"Team `{teamDto.Name}` updated @ {DateTime.Now}");

            return Ok();
        }

        [HttpDelete, Route("{id:long}")]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            var user = HttpContext.Items["user"] as User;
            
            await _teamService.DeleteTeamAsync(id, user);
            _logger.LogInformation($"Team with id `{id}` deleted @ {DateTime.Now}");

            return NoContent();
        }
    }
}