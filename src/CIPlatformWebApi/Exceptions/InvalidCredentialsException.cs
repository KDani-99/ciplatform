using System.Net;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Exceptions
{
    public class InvalidCredentialsException : BadHttpRequestException
    {
        public InvalidCredentialsException() : base("Invalid username or password.", (int) HttpStatusCode.Unauthorized)
        {
        }
    }
}