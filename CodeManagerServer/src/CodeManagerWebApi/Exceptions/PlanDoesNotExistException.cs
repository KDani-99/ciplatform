using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class PlanDoesNotExistException : BadHttpRequestException
    {
        public PlanDoesNotExistException() : base("The specified plan does not exist.", (int) HttpStatusCode.NotFound)
        {
        }
    }
}