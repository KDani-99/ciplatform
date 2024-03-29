﻿using System.Net;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Exceptions
{
    public class ProjectDoesNotExistException : BadHttpRequestException
    {
        public ProjectDoesNotExistException() : base("The specified project does not exist.",
                                                     (int) HttpStatusCode.BadRequest)
        {
        }
    }
}