using System;
using System.Net;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Exceptions;
using CodeManagerWebApi.Services;
using MassTransit.Futures.Contracts;
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
        private readonly IProjectService _projectService;
        private readonly IVariableService _variableService;
        private readonly ILogger<ProjectController> _logger;
        
        public ProjectController(IProjectService projectService, IVariableService variableService, ILogger<ProjectController> logger)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _variableService = variableService ?? throw new ArgumentNullException(nameof(variableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var project = await _projectService.GetProjectsAsync();
            return Ok(project);
        }
        
        [HttpGet, Route("{id:long}", Name = nameof(GetProject))]
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

            return CreatedAtRoute( nameof(GetProject), new {result.Id}, result);
        }
        
        [HttpPut, Route("{id:long}")]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] CreateProjectDto projectDto)
        {
            var user = HttpContext.Items["user"] as User;
            await _projectService.UpdateProjectAsync(id, projectDto, user);
            _logger.LogInformation($"Project `{projectDto.Name}` has been updated @ {DateTime.Now}");

            return Ok();
        }
        
        [HttpDelete, Route("{id:long}")]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            var user = HttpContext.Items["user"] as User;
            
            await _projectService.DeleteProjectAsync(id, user);
            _logger.LogInformation($"Project with id `{id}` has been deleted @ {DateTime.Now}");

            return NoContent();
        }

        [HttpPut, Route("{id:long}/vars")] // constant long param
        public async Task<IActionResult> PutVariable([FromRoute] long id, [FromBody] VariableDto variableDto)
        {
            // Logically, a variable cant exist without a project, so that's the reason why I decided not to make a separate controller for that
            // TODO: verify user permission
            var user = HttpContext.Items["user"] as User;
            await _variableService.CreateOrUpdateVariableAsync(id, variableDto, user);

            return NoContent();
        }
        
        
    }
}