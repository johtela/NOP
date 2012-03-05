namespace NOP
{
	using System;
	using NOP.Collections;

	public class MethodCallExpression : ListExpression
	{
		public readonly Expression ObjectExpression;
		public readonly Method CalledMethod;
		public readonly List<Expression> Parameters;
		
		public MethodCallExpression (SList methodExpr) : base (methodExpr)
		{
			var sexps = methodExpr.Items;
			ObjectExpression = Parse (Expect<SExpr> (ref sexps, "object reference"));
			CalledMethod = Expect<Method> (ref sexps, "method");

			var paramCount = CalledMethod.MethodInfo.GetParameters().Length;
			if (sexps.Length != paramCount)
				ParseError (methodExpr, string.Format(
					"Wrong number of parameters given to the method. " +
					"Expected {0}, found {1}", paramCount, sexps.Length));
			Parameters = sexps.Map (sexp => Parse (sexp));
		}
	}
}

