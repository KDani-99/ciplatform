using System;

namespace CodeManagerAgent.Exceptions
{
    public class EnvironmentNotPreparedException : Exception
    {
        public EnvironmentNotPreparedException(string message) : base(message) {}
    }
}