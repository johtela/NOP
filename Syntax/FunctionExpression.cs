namespace NOP
{
	using System;
	using Collections;
	
	public class FunctionExpression : ListExpression
	{
		public readonly Expression FuncName;
		public readonly List<Expression> Parameters;
		
		public FunctionExpression (SList funcExpr) : base (funcExpr)
		{
			var sexps = funcExpr.Items;
			FuncName = Parse (Expect<SExpr> (ref sexps, "function name or lambda expression"));
			Parameters = sexps.Map (sexp => Parse(sexp));
		}
	}
}

