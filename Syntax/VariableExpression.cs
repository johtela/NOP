namespace NOP
{
	using System;

	public class VariableExpression : Expression
	{
		public readonly SymbolExpression Name;
		public readonly TypeExpression Type;

		public VariableExpression (SExpr sexp) : base (sexp)
		{
			Type = null;
			var sym = sexp as SExpr.Symbol;

			if (sym != null)
				Name = new SymbolExpression (sym);
			else
			{
				var lst = sexp as SExpr.List;

				if (lst != null)
				{
					var sexps = lst.Items;
					Name = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "variable name"));
					Type = TypeExpression.ParseTypeExpression (Expect<SExpr> (ref sexps, "type expression"));
				}
			}
		}
	}
}

