namespace NOP
{
	using System;
	using NOP.Collections;

	public class MemberDefinition : Definition
	{
		public readonly VariableDefinition Variable;
		public readonly Expression Value;

		public MemberDefinition (SExpr.List typeDef) : base (typeDef)
		{
			var sexps = typeDef.Items.Rest;
			Variable = new VariableDefinition (Expect<SExpr> (ref sexps, "member name"));
			Value = Expression.Parse (sexps.First);
		}
	}
}

