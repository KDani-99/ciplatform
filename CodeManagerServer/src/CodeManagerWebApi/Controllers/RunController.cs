using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.Exceptions;
using CodeManagerWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeManagerWebApi.Controllers
{
    [ApiController]
    [Route("/api/runs")]
    [Authorize(Roles = "User,Admin")]
    public class RunController : ControllerBase
    {
        private readonly IRunService _runService;

        public RunController(IRunService runService)
        {
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
        }

        [HttpGet]
        [Route("{runId:long}")]
        public async Task<IActionResult> GetRun([FromRoute] long runId)
        {
            var user = HttpContext.Items["user"] as User;
            var result = await _runService.GetRunAsync(runId, user);

            return Ok(result);
        }

        [HttpGet]
        [Route("{runId:long}/{jobId:long}")]
        public async Task<IActionResult> GetJob([FromRoute] long runId, [FromRoute] long jobId)
        {
            var user = HttpContext.Items["user"] as User;
            var result = await _runService.GetJobAsync(runId, jobId, user);

            return Ok(result);
        }

        [HttpGet]
        [Route("{runId:long}/{jobId:long}/{stepId:long}")]
        public async Task<IActionResult> GetStepFile([FromRoute] long runId,
                                                     [FromRoute] long jobId,
                                                     [FromRoute] long stepId)
        {
            var user = HttpContext.Items["user"] as User;
            var stream = await _runService.GetStepFileStreamAsync(runId, jobId, stepId, user);

            return File(stream, "text/plain");
        }

        [HttpPost]
        [Route("{projectId:long}/start")]
        public async Task<IActionResult> StartRun([FromRoute] long projectId)
        {
            var user = HttpContext.Items["user"] as User;
            var file = Request.Form.Files.FirstOrDefault() ?? throw new InvalidInstructionFileException();

            using var streamReader = new StreamReader(file.OpenReadStream());
            var instructions = await streamReader.ReadToEndAsync();

            await _runService.QueueRunAsync(projectId, instructions, user);

            return Ok();
        }
    }
}