using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class UsernameTakenException : BadHttpRequestException
    {
        public UsernameTakenException() : base("The specified username is already in use.",
                                               (int) HttpStatusCode.Conflict)
        {
        }
    }
}