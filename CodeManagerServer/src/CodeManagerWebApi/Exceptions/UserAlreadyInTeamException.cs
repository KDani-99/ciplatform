using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class UserAlreadyInTeamException : BadHttpRequestException
    {
        public UserAlreadyInTeamException() : base("You are already in this team.", (int)HttpStatusCode.Conflict){}
    }
}