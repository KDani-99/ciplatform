using System.Net;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Exceptions
{
    public class UserDoesNotExistException : BadHttpRequestException
    {
        public UserDoesNotExistException() : base("The specified user does not exist.", (int) HttpStatusCode.BadRequest)
        {
        }
    }
}