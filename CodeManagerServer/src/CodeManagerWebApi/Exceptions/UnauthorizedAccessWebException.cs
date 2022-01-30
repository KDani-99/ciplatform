﻿using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class UnauthorizedAccessWebException : BadHttpRequestException
    {
        public UnauthorizedAccessWebException(string message) : base(message, (int)HttpStatusCode.Unauthorized) {}
    }
}