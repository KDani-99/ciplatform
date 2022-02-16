using System.Net;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Exceptions
{
    public class TeamAlreadyExistsException : BadHttpRequestException
    {
        public TeamAlreadyExistsException() : base("The specified team name is already in use.",
                                                   (int) HttpStatusCode.Conflict)
        {
        }
    }
}