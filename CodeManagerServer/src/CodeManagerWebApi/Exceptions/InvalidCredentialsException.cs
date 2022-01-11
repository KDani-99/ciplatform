﻿using System;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class InvalidCredentialsException : BadHttpRequestException
    {
        public InvalidCredentialsException() : base ("Invalid username or password.") {}
    }
}