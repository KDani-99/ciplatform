using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class RunDoesNotExistException : BadHttpRequestException
    {
        public RunDoesNotExistException() : base("The specified run does not exist.", (int) HttpStatusCode.BadRequest)
        {
        }
    }
}