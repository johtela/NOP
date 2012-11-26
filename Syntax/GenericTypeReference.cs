namespace NOP
{
	using System;
	using NOP.Collections;

	public class GenericTypeReference : TypeReference
	{
		public readonly SymbolExpression TypeName;
		public readonly List<TypeReference> TypeParams;

		public GenericTypeReference (SExpr.List typeExpr) : base (typeExpr)
		{
			TypeName = new SymbolExpression ((SExpr.Symbol)typeExpr.Items.First);
			TypeParams = typeExpr.Items.Rest.Map (sexp => ParseTypeExpression (sexp));
		}
	}
}

