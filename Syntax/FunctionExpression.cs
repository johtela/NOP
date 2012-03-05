namespace NOP
{
	using System;
	
	public class FunctionExpression : AtomExpression
	{
		public readonly Function Function;
		
		public FunctionExpression (Function function) : base (function)
		{
			Function = function;
		}
	}
}

