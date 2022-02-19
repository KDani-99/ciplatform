using System.Net;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Exceptions
{
    public class JobDoesNotExistException : BadHttpRequestException
    {
        public JobDoesNotExistException() : base("The specified job does not exist.", (int) HttpStatusCode.BadRequest)
        {
        }
    }
}