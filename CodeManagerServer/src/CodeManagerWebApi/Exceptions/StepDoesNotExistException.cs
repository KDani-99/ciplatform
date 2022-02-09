using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class StepDoesNotExistException : BadHttpRequestException
    {
        public StepDoesNotExistException() : base("The specified step does not exist.", (int) HttpStatusCode.BadRequest)
        {
        }
    }
}