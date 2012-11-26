namespace NOP
{
	using System;
	using NOP.Collections;

	public abstract class TypeReference : AstNode
	{
		public TypeReference (SExpr sexp) : base (sexp)
		{
		}

		public static TypeReference ParseTypeExpression (SExpr sexp)
		{
			var lst = sexp as SExpr.List;

			if (lst != null)
			{
				var sexps = lst.Items;
				if (sexps.IsEmpty || !(sexps.First is SExpr.Symbol))
					ParseError (lst, "Expected a symbol");
				var typeName = sexps.First as SExpr.Symbol;

				return typeName.Name == "->" ? 
					(TypeReference)new LambdaTypeReference (lst) : 
					(TypeReference)new GenericTypeReference (lst);
			}
			var sym = sexp as SExpr.Symbol;
			if (sym == null)
				ParseError (sexp, "Expected a type expression");
			return new BasicTypeReference (sym);
		}
	}
}

