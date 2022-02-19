using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Project;
using CIPlatformWebApi.Extensions.Utils;
using CIPlatformWebApi.Services;
using CIPlatformWebApi.Services.Project;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CIPlatformWebApi.Controllers
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
        public async Task<IActionResult> GetProjectsAsync()
        {
            try
            {
                var project = await _projectService.GetProjectsAsync();
                return Ok(project);
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpGet]
        [Route("{id:long}", Name = nameof(GetProjectAsync))]
        public async Task<IActionResult> GetProjectAsync([FromRoute] long id)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                var project = await _projectService.GetProjectAsync(id, user);

                return Ok(project);
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectAsync([FromBody] CreateProjectDto createProjectDto)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                var result = await _projectService.CreateProjectAsync(createProjectDto, user);
                _logger.LogInformation($"Project `{createProjectDto.Name}` has been created @ {DateTime.Now}");

                return CreatedAtRoute(nameof(GetProjectAsync), new { result.Id }, result);
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpPut]
        [Route("{id:long}")]
        public async Task<IActionResult> UpdateProjectAsync([FromRoute] long id, [FromBody] CreateProjectDto projectDto)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                await _projectService.UpdateProjectAsync(id, projectDto, user);
                _logger.LogInformation($"Project `{projectDto.Name}` has been updated @ {DateTime.Now}");

                return Ok();
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> DeleteProjectAsync([FromRoute] long id)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;

                await _projectService.DeleteProjectAsync(id, user);
                _logger.LogInformation($"Project with id `{id}` has been deleted @ {DateTime.Now}");

                return NoContent();
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }
    }
}