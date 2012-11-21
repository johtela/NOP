namespace NOP
{
	using System;
	using NOP.Collections;

	public abstract class TypeExpression : Expression
	{
		public TypeExpression (SExpr sexp) : base (sexp)
		{
		}

		public static TypeExpression ParseTypeExpression (SExpr sexp)
		{
			var lst = sexp as SExpr.List;

			if (lst != null)
			{
				var sexps = lst.Items;
				if (sexps.IsEmpty || !(sexps.First is SExpr.Symbol))
					ParseError (lst, "Expected a symbol");
				var typeName = sexps.First as SExpr.Symbol;

				return typeName.Name == "->" ? 
					(TypeExpression)new LambdaTypeExpression (lst) : 
					(TypeExpression)new GenericTypeExpression (lst);
			}
			var sym = sexp as SExpr.Symbol;
			if (sym == null)
				ParseError (sexp, "Expected a type expression");
			return new BasicTypeExpression (sym);
		}
	}
}

