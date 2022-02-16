using System;

namespace CIPlatformManager.Exceptions
{
    public class LogStreamException : Exception
    {
        public LogStreamException(string message) : base(message)
        {
        }
    }
}