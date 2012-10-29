namespace NOP
{
	using System;
	using NOP.Collections;

	public class DefinitionExpression : Expression
	{
		public DefinitionExpression (SExpr.List memberExpr) : base(memberExpr)
		{
		}
	}
}

