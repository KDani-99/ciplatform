using System;
using System.Threading.Tasks;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TeamDto teamDto)
        {
            var user = HttpContext.Items["user"] as User;
            await _teamService.CreateTeam(teamDto, user);
            return StatusCode(201);
        }
    }
}