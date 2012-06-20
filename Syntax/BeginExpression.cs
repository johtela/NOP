namespace NOP
{
	using System;
	using Collections;

	public class BeginExpression : ListExpression
	{
		public readonly List<Expression> Expressions;
		
		public BeginExpression (SExpr.List beginExpr) : base (beginExpr)
		{
			var sexps = beginExpr.Items.Rest;
			if (sexps.IsEmpty)
				ParseError (beginExpr, "Expected at least one expression after begin");
			Expressions = sexps.Map (sexp => Parse (sexp));
		}
	}
}