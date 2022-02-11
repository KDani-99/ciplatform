using System;

namespace CodeManagerAgentManager.Exceptions
{
    public class WorkerConnectionCacheException : Exception
    {
        public WorkerConnectionCacheException(string message) : base(message)
        {
        }
    }
}