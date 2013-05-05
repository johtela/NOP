namespace NOP
{
	using System;

	public class ParseError : Exception
	{
		public ParseError (string msg) : base (msg)
		{
		}
	}
}

