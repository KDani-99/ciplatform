using System;

namespace CodeManagerAgent.Exceptions
{
    public class StepFailedException : Exception
    {
        public string Name { get; set; }
        public long ExitCode { get; set; }
        
        public StepFailedException() {}
        
        public StepFailedException(string message) : base(message) {}
    }
}