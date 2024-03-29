﻿using System.Net;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Exceptions
{
    public class EmailAlreadyInUseException : BadHttpRequestException
    {
        public EmailAlreadyInUseException() : base("Email address is already in use.", (int) HttpStatusCode.Conflict)
        {
        }
    }
}