namespace NOP
{
	using System;

	public class Literal : SExpr
	{
		public readonly object Value;
		
		public Literal (object value)
		{
			Value = value;
		}
	}
}

