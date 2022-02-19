using System.Net;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Exceptions
{
    public class TeamDoesNotExistException : BadHttpRequestException
    {
        public TeamDoesNotExistException() : base("The specified team does not exist.", (int) HttpStatusCode.BadRequest)
        {
        }
    }
}