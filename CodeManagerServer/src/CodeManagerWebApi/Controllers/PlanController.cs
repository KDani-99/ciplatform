using System;
using System.Net;
using System.Threading.Tasks;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeManagerWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("/api/plans")]
    [Produces("application/json")]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;
        private readonly ILogger<PlanController> _logger;

        public PlanController(IPlanService planService, ILogger<PlanController> logger)
        {
            _planService = planService ?? throw new ArgumentNullException(nameof(planService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlanDto planDto)
        {
            await _planService.CreatePlanAsync(planDto);
            _logger.LogInformation($"Plan `{planDto.Name}` created @ {DateTime.Now}");
            
            return StatusCode((int)HttpStatusCode.Created);
        }
        
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long id)
        {
            await _planService.DeletePlanAsync(id);
            _logger.LogInformation($"Plan with id `{id}` deleted @ {DateTime.Now}");
            
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}