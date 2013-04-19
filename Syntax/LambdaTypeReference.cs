namespace NOP
{
	using System;
	using System.Collections.Generic;
	using NOP.Collections;

	public class LambdaTypeReference : TypeReference
	{
		public readonly TypeReference ArgumentType, ResultType;

		public LambdaTypeReference (SExpr.List typeExpr) : base (typeExpr)
		{
			var sexps = typeExpr.Items.RestL;
			ArgumentType = ParseTypeExpression (Expect<SExpr> (ref sexps, "type"));
			ResultType = ParseTypeExpression (Expect<SExpr> (ref sexps, "type"));
		}

		protected override void DoForChildNodes (Action<AstNode> action)
		{
			action (ArgumentType);
			action (ResultType);
		}
	}
}

