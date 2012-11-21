namespace NOP
{
	using System;
	using NOP.Collections;

	public class GenericTypeExpression : TypeExpression
	{
		public readonly SymbolExpression TypeName;
		public readonly List<TypeExpression> TypeParams;

		public GenericTypeExpression (SExpr.List typeExpr) : base (typeExpr)
		{
			TypeName = new SymbolExpression ((SExpr.Symbol)typeExpr.Items.First);
			TypeParams = typeExpr.Items.Rest.Map (sexp => ParseTypeExpression (sexp));
		}
	}
}

