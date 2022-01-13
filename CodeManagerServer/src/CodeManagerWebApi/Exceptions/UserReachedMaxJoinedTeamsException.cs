using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class UserReachedMaxJoinedTeamsException : BadHttpRequestException
    {
        public UserReachedMaxJoinedTeamsException() : base("You have reached the maximum number of teams that you can join.", (int) HttpStatusCode.Forbidden) {}
    }
}