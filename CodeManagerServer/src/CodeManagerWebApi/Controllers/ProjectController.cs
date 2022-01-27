using System;
using System.Net;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeManagerWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
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
            await _projectService.CreateProjectAsync(createProjectDto);
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut, Route("{projectId:long}/vars")] // constant long param
        public async Task<IActionResult> CreateOrUpdateProjectVariableAsync([FromRoute] long projectId,[FromBody] VariableDto variableDto)
        {
            // Logically, a variable cant exist without a project, so thats the reason why I decided not to make a separate controller for that
            // TODO: validate dto
            // TODO: verify user permission

            var project = await _projectService.GetProjectAsync(projectId);
            await _variableService.CreateOrUpdateVariableAsync(project, variableDto);
            
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}