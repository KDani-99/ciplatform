using System.Net;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Exceptions
{
    public class ProjectAlreadyExistsException : BadHttpRequestException
    {
        public ProjectAlreadyExistsException() : base("The specified project name is already in use in this project.",
                                                      (int) HttpStatusCode.Conflict)
        {
        }
    }
}