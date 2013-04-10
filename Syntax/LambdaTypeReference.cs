namespace NOP
{
	using System;
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

		protected override System.Collections.Generic.IEnumerable<AstNode> GetChildNodes ()
		{
			return List.Create<AstNode> (ArgumentType, ResultType);
		}
	}
}

