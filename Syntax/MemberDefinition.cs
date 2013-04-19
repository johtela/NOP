namespace NOP
{
	using System;
	using NOP.Collections;
	using System.Collections.Generic;

	public class MemberDefinition : Definition
	{
		public readonly VariableDefinition Variable;
		public readonly Expression Value;

		public MemberDefinition (SExpr.List typeDef) : base (typeDef)
		{
			var sexps = typeDef.Items.RestL;
			Variable = new VariableDefinition (Expect<SExpr> (ref sexps, "member name"));
			Value = Expression.Parse (sexps.First);
		}

		protected override void DoForChildNodes (Action<AstNode> action)
		{
			action (Variable);
			action (Value);
		}
	}
}

