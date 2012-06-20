namespace NOP
{
	using System;
	using Collections;
	
	public class ApplicationExpression : ListExpression
	{
		public readonly Expression FuncName;
		public readonly List<Expression> Parameters;
		
		public ApplicationExpression (SExpr.List funcExpr) : base (funcExpr)
		{
			var sexps = funcExpr.Items;
			FuncName = Parse (Expect<SExpr> (ref sexps, "function name or lambda expression"));
			Parameters = sexps.Map (sexp => Parse(sexp));
		}
	}
}

