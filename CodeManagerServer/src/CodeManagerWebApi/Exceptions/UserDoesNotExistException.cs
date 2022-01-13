using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class UserDoesNotExistException : BadHttpRequestException
    {
        public UserDoesNotExistException() : base("The specified user does not exist.", (int)HttpStatusCode.NotFound) {}
    }
}