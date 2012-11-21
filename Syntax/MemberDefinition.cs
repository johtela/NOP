namespace NOP
{
	using System;
	using NOP.Collections;

	public class MemberDefinition : Definition
	{
		public readonly VariableExpression Name;
		public readonly Expression Value;

		public MemberDefinition (SExpr typeDef) : base (typeDef)
		{
		}
	}
}

