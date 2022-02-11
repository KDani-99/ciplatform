using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Utils.Extensions
{
    public static class HttpExceptionHandler
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder appBuilder)
        {
            appBuilder.Run(async context =>
            {
                var exception = context.Features
                                       .Get<IExceptionHandlerPathFeature>()
                                       .Error;

                if (!context.Response.HasStarted)
                {
                    if (exception is BadHttpRequestException ex)
                    {
                        var response = new
                        {
                            error = ex.Message,
                            status = ex.StatusCode
                        };

                        context.Response.StatusCode = ex.StatusCode;

                        await context.Response.WriteAsJsonAsync(response);
                    }
                    else
                    {
                        var response = new
                        {
                            error = "An unexpected error has occured.",
                            status = (int) HttpStatusCode.InternalServerError
                        };
                        await context.Response.WriteAsJsonAsync(response);
                    }
                }
            });

            return appBuilder;
        }
    }
}