using System;

namespace CodeManagerWebApi.Exceptions
{
    public class ChannelDoesNotExistException : Exception
    {
        public ChannelDoesNotExistException(string channel) : base($"The specified channel `{channel}` does not exist.")
        {
        }
    }
}