namespace NOP
{
	using System;
	using NOP.Collections;

	public class DefinitionExpression : ListExpression
	{
		public DefinitionExpression (SExpr.List memberExpr) : base(memberExpr)
		{
		}
	}
}

