using System.Net;
using Microsoft.AspNetCore.Http;

namespace CIPlatformWebApi.Exceptions
{
    public class PlanAlreadyExistsException : BadHttpRequestException
    {
        public PlanAlreadyExistsException() : base("The specified plan name is already in use..",
                                                   (int) HttpStatusCode.UnprocessableEntity)
        {
        }
    }
}