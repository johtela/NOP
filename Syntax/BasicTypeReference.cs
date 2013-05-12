namespace NOP
{
	using System;
	using System.Collections.Generic;
	using NOP.Collections;

	public class BasicTypeReference : TypeReference
	{
		public readonly SymbolExpression TypeName;

		public BasicTypeReference (SExpr sexp, SymbolExpression typeName)
			: base (sexp)
		{
			TypeName = typeName;
		}

		public BasicTypeReference (SExpr.Symbol typeName) : base (typeName)
		{
			TypeName = new SymbolExpression (typeName);
		}

		protected override void DoForChildNodes (Action<AstNode> action)
		{
			action (TypeName);
		}
	}
}

