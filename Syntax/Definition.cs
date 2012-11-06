namespace NOP
{
	using System;
	using NOP.Collections;

	public abstract class Definition : AstNode
	{
		public readonly SymbolExpression Name;

		public Definition (SExpr.List defExpr) : base (defExpr)
		{
			var sexps = defExpr.Items.Rest;
			Name = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "definition name"));
		}
	}
}

