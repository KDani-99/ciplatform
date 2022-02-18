using System;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CIPlatformWebApi.Controllers
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
                    Error = ex.Message,
                    Status = ex.StatusCode
                };
                return StatusCode(ex.StatusCode, response);
            }
            else
            {
                var response = new
                    { Error = "An unexpected error has occured.", Status = (int) HttpStatusCode.InternalServerError };
                _logger.LogTrace(exception, $"An unexpected error has occrred @ {DateTime.Now}.");
                return StatusCode((int) HttpStatusCode.InternalServerError, response);
            }
        }
    }
}