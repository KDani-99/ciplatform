using System;

namespace CodeManagerAgentManager.Exceptions
{
    public class LogStreamException : Exception
    {
        public LogStreamException(string message) : base(message) {}
    }
}