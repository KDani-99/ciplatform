using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class UserNotActivatedException : BadHttpRequestException
    {
        public UserNotActivatedException() : base("Account has not been activated yet.", (int) HttpStatusCode.Forbidden) {}
    }
}