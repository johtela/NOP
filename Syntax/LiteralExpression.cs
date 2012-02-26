namespace NOP
{
	using System;

	public class LiteralExpression : AtomExpression
	{
		public readonly Literal Literal;
		
		public LiteralExpression (Literal literal) : base (literal)
		{
			Literal = literal;
		}
	}
}