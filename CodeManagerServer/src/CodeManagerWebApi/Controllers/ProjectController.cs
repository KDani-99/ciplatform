using System;
using System.Net;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Exceptions;
using CodeManagerWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        
        public ProjectController(IProjectService projectService, IVariableService variableService)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _variableService = variableService ?? throw new ArgumentNullException(nameof(variableService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectAsync([FromBody] CreateProjectDto createProjectDto)
        {
            var user = HttpContext.Items["user"] as User;
            await _projectService.CreateProjectAsync(createProjectDto, user);
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpGet, Route("{projectId:long}")]
        public async Task<IActionResult> GetProjectAsync([FromRoute] long projectId)
        {
            var user = HttpContext.Items["user"] as User;
            return Ok(await _projectService.GetProjectAsync(projectId, user));
        }

        [HttpPut, Route("{projectId:long}/vars")] // constant long param
        public async Task<IActionResult> CreateOrUpdateProjectVariableAsync([FromRoute] long projectId,[FromBody] VariableDto variableDto)
        {
            // Logically, a variable cant exist without a project, so that's the reason why I decided not to make a separate controller for that
            // TODO: verify user permission
            var user = HttpContext.Items["user"] as User;

            var project = await _projectService.GetProjectAsync(projectId, user);
            await _variableService.CreateOrUpdateVariableAsync(project, variableDto);
            
            return StatusCode((int)HttpStatusCode.NoContent);
        }
        
        
    }
}