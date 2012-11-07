namespace NOP
{
	using System;
	using NOP.Collections;

	public abstract class Definition : AstNode
	{
		public readonly SymbolExpression Name;

		public Definition (List<SExpr> definition) : base (definition.First)
		{
			var sexps = definition.Rest;
			Name = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "definition name"));
		}

		public static Definition Parse (SExpr sexp)
		{
			var lst = sexp as SExpr.List;
			if (lst == null)
				ParseError (sexp, "Expected a definition beginning with a list");

			var sexps = lst.Items;
			var keyword = sexps.First as SExpr.Symbol;
			if (sexps == null)
				ParseError (sexps.First, "Expected a symbol");

			// Check if we have any of the special forms as first item.
			switch (keyword.Name)
			{
				case "module":
					return new ModuleDefinition (sexps);
			}
			// TODO: Left here!
			return null;
		}
	}
}