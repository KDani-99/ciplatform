using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeManagerWebApi.Controllers
{
    [ApiController]
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult HandleError()
        {
            var exception = HttpContext.Features
                .Get<IExceptionHandlerPathFeature>()
                .Error;
            
            if (exception is BadHttpRequestException ex)
            {
                var response = new
                {
                    error = ex.Message,
                    status = ex.StatusCode
                };
                return StatusCode(ex.StatusCode, response);
            }
            else
            {
                var response = new { error = "An unexpected error has occured.", status = (int) HttpStatusCode.InternalServerError };
                _logger.LogTrace(exception, $"An unexpected error has occured @ {DateTime.Now}.");
                return StatusCode((int) HttpStatusCode.InternalServerError, response);
            }
            
        }
    }
}