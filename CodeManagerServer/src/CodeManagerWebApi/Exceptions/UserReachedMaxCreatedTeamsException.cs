using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class UserReachedMaxCreatedTeamsException : BadHttpRequestException
    {
        public UserReachedMaxCreatedTeamsException() : base("You have reached the maximum number of teams that you can create with the current plan.", (int) HttpStatusCode.Forbidden) {}
    }
}