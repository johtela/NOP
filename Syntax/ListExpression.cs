namespace NOP
{
	using System;

	public abstract class ListExpression : Expression
	{
		public ListExpression (SExpr.List list) : base (list)
		{
		}
	}
}

