using System;

namespace CIPlatformWorker.Exceptions
{
    public class EnvironmentNotPreparedException : Exception
    {
        public EnvironmentNotPreparedException(string message) : base(message)
        {
        }
    }
}