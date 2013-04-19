namespace NOP
{
	using System;
	using NOP.Collections;
	using System.Collections.Generic;

	public class GenericTypeReference : TypeReference
	{
		public readonly SymbolExpression TypeName;
		public readonly NOPList<TypeReference> TypeParams;

		public GenericTypeReference (SExpr.List typeExpr) : base (typeExpr)
		{
			TypeName = new SymbolExpression ((SExpr.Symbol)typeExpr.Items.First);
			TypeParams = List.MapReducible (typeExpr.Items.RestL, sexp => ParseTypeExpression (sexp));
		}

		protected override void DoForChildNodes (Action<AstNode> action)
		{
			action (TypeName);
			TypeParams.Foreach (action);
		}
	}
}