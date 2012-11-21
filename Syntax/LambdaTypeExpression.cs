namespace NOP
{
	using System;
	using NOP.Collections;

	public class LambdaTypeExpression : TypeExpression
	{
		public readonly TypeExpression ArgumentType, ResultType;

		public LambdaTypeExpression (SExpr.List typeExpr) : base (typeExpr)
		{
			var sexps = typeExpr.Items.Rest;
			ArgumentType = ParseTypeExpression (Expect<SExpr> (ref sexps, "type"));
			ResultType = ParseTypeExpression (Expect<SExpr> (ref sexps, "type"));
		}
	}
}

