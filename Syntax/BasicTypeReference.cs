namespace NOP
{
	using System;

	public class BasicTypeReference : TypeReference
	{
		public readonly SymbolExpression TypeName;

		public BasicTypeReference (SExpr.Symbol typeName) : base (typeName)
		{
			TypeName = new SymbolExpression (typeName);
		}
	}
}

