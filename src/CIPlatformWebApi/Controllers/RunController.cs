using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Extensions.Utils;
using CIPlatformWebApi.Services;
using CIPlatformWebApi.Services.Run;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CIPlatformWebApi.Controllers
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
        
        [HttpGet, Route("{runId:long}", Name = nameof(GetRunAsync))]
        public async Task<IActionResult> GetRunAsync([FromRoute] long runId)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                var result = await _runService.GetRunDataAsync(runId, user);

                return Ok(result);
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpGet, Route("{runId:long}/data")]
        public async Task<IActionResult> GetRunDataAsync([FromRoute] long runId)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                var result = await _runService.GetRunDataAsync(runId, user);

                return Ok(result);
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpGet, Route("{runId:long}/{jobId:long}")]
        public async Task<IActionResult> GetJobAsync([FromRoute] long runId, [FromRoute] long jobId)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                var result = await _runService.GetJobAsync(runId, jobId, user);

                return Ok(result);
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpGet, Route("{runId:long}/{jobId:long}/{stepId:long}")]
        public async Task<IActionResult> GetStepFileAsync([FromRoute] long runId,
                                                     [FromRoute] long jobId,
                                                     [FromRoute] long stepId)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                var stream = await _runService.GetStepFileStreamAsync(runId, jobId, stepId, user);

                return File(stream, "text/plain");
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpPost, Route("{projectId:long}/create")]
        public async Task<IActionResult> StartRunAsync([FromRoute] long projectId)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                var file = Request.Form.Files.FirstOrDefault() ?? throw new InvalidInstructionFileException();

                using var streamReader = new StreamReader(file.OpenReadStream());
                var instructions = await streamReader.ReadToEndAsync();

                var result = await _runService.CreateRunAsync(projectId, instructions, user);

                return CreatedAtRoute(nameof(GetRunAsync), new { RunId = result.Id }, result);
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }

        [HttpDelete, Route("{runId:long}")]
        public async Task<IActionResult> DeleteRunAsync([FromRoute] long runId)
        {
            try
            {
                var user = HttpContext.Items["user"] as UserEntity;
                await _runService.DeleteRunAsync(runId, user);

                return NoContent();
            }
            catch (BadHttpRequestException exception)
            {
                return StatusCode(exception.StatusCode, exception.ToErrorResponse());
            }
        }
    }
}