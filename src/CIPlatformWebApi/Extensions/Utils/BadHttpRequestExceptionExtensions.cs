using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Extensions.Utils
{
    public static class BadHttpRequestExceptionExtensions
    {
        public static object ToErrorResponse(this BadHttpRequestException exception)
        {
            return new
            {
                Status = exception.StatusCode,
                exception.Message
            };
        }
    }
}