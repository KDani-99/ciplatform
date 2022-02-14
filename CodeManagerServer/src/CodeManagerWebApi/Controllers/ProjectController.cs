using System;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeManagerWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    [Route("/api/projects")]
    [Produces("application/json")]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService, ILogger<ProjectController> logger)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var project = await _projectService.GetProjectsAsync();
            return Ok(project);
        }

        [HttpGet]
        [Route("{id:long}", Name = nameof(GetProject))]
        public async Task<IActionResult> GetProject([FromRoute] long id)
        {
            var user = HttpContext.Items["user"] as User;
            var project = await _projectService.GetProjectAsync(id, user);

            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProjectDto createProjectDto)
        {
            var user = HttpContext.Items["user"] as User;
            var result = await _projectService.CreateProjectAsync(createProjectDto, user);
            _logger.LogInformation($"Project `{createProjectDto.Name}` has been created @ {DateTime.Now}");

            return CreatedAtRoute(nameof(GetProject), new { result.Id }, result);
        }

        [HttpPut]
        [Route("{id:long}")]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] CreateProjectDto projectDto)
        {
            var user = HttpContext.Items["user"] as User;
            await _projectService.UpdateProjectAsync(id, projectDto, user);
            _logger.LogInformation($"Project `{projectDto.Name}` has been updated @ {DateTime.Now}");

            return Ok();
        }

        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            var user = HttpContext.Items["user"] as User;

            await _projectService.DeleteProjectAsync(id, user);
            _logger.LogInformation($"Project with id `{id}` has been deleted @ {DateTime.Now}");

            return NoContent();
        }
    }
}