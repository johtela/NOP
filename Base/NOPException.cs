using System;

namespace NOP
{
    // The general exception class.
    public class NOPException : Exception
    {
        public NOPException(string message) : base(message)
        { }
    }
}
