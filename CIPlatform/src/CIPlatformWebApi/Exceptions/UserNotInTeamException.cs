using System.Net;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Exceptions
{
    public class UserNotInTeamException : BadHttpRequestException
    {
        public UserNotInTeamException() : base("You must be in the team to kick members.",
                                               (int) HttpStatusCode.BadRequest)
        {
        }

        public UserNotInTeamException(string msg) : base(msg, (int) HttpStatusCode.BadRequest)
        {
        }
    }
}