using System;

namespace CIPlatformWorker.Exceptions
{
    public class StepFailedException : Exception
    {
        public StepFailedException()
        {
        }

        public StepFailedException(string message) : base(message)
        {
        }

        public string Name { get; set; }
        public long ExitCode { get; set; }
    }
}