namespace NOP
{
	using System;
	
	public class ValueExpression : AtomExpression
	{
		public readonly Value Value;
		
		public ValueExpression (Value value) : base (value)
		{
			Value = value;
		}
	}
}

