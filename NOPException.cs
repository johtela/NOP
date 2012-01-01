using System;

namespace NOP
{
    public class NOPException : Exception
    {
        public NOPException(string message) : base(message)
        { }
    }
}
