namespace NOP
{
	using System;

	public class VariableDefinition : AstNode
	{
		public readonly SymbolExpression Name;
		public readonly TypeReference Type;

		public VariableDefinition (SExpr sexp) : base (sexp)
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
					Type = TypeReference.ParseTypeExpression (Expect<SExpr> (ref sexps, "type expression"));
				}
			}
		}
	}
}

