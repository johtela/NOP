namespace NOP
{
	using System;
	using NOP.Collections;

	public class LambdaTypeReference : TypeReference
	{
		public readonly TypeReference ArgumentType, ResultType;

		public LambdaTypeReference (SExpr.List typeExpr) : base (typeExpr)
		{
			var sexps = typeExpr.Items.Rest;
			ArgumentType = ParseTypeExpression (Expect<SExpr> (ref sexps, "type"));
			ResultType = ParseTypeExpression (Expect<SExpr> (ref sexps, "type"));
		}
	}
}

