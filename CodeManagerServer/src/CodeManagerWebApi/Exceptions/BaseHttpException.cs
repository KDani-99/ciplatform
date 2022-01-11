using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public abstract class BaseHttpException : BadHttpRequestException
    {
        protected BaseHttpException(string message, int statusCode) : base(message, statusCode) {}
    }
}