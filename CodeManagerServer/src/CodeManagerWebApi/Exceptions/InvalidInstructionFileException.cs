using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Exceptions
{
    public class InvalidInstructionFileException : BadHttpRequestException
    {
        public InvalidInstructionFileException() : base("Instructions file is invalid or missing.",
                                                        (int) HttpStatusCode.BadRequest)
        {
        }
        public InvalidInstructionFileException(string msg) : base(msg,
                                                        (int) HttpStatusCode.BadRequest)
        {
        }
    }
}