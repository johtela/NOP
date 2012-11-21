namespace NOP
{
	using System;
	using NOP.Collections;

	public abstract class Definition : AstNode
	{
		public Definition (SExpr sexp) : base (sexp)
		{
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
				case "type": 
					return new TypeDefinition (lst);
				case "def": 
					return new MemberDefinition (lst);
			}
			ParseError (keyword, "Expected keyword 'type' or 'def'");
			return null;
		}
	}
}