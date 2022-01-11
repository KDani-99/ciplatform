using System;

namespace CodeManagerWebApi.Exceptions
{
    public class UsernameTakenException : Exception
    {
        public UsernameTakenException() : base("Username is already taken.") {}
    }
}