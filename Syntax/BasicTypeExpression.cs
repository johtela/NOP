namespace NOP
{
	using System;

	public class BasicTypeExpression : TypeExpression
	{
		public readonly SymbolExpression TypeName;

		public BasicTypeExpression (SExpr.Symbol typeName) : base (typeName)
		{
			TypeName = new SymbolExpression (typeName);
		}
	}
}

