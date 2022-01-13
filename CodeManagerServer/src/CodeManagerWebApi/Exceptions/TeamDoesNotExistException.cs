using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class TeamDoesNotExistException : BadHttpRequestException
    {
        public TeamDoesNotExistException() : base("The specified team does not exist.", (int)HttpStatusCode.NotFound) {}
    }
}