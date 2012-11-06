namespace NOP
{
	using System;
	using Collections;

	/// <summary>
	/// Abstract class representing any language expression. The root class of the 
	/// abstract syntax tree.
	/// </summary>
	public abstract class Expression : AstNode
	{
		public Expression (SExpr sexp) : base (sexp)
		{ }
		
		/// <summary>
		/// Get the type expression that is used to type check this expression.
		/// </summary>
		public virtual TypeExpr GetTypeExpr ()
		{
			TypeExpr.CurrentExpression = this;
			return null;
		}
				
		/// <summary>
		/// Parse an S-expression and generate the AST.
		/// </summary>
		public static Expression Parse (SExpr sexp)
		{
			if (sexp is SExpr.Symbol)
				return new SymbolExpression (sexp as SExpr.Symbol);
			var slist = sexp as SExpr.List;
			if (slist != null)
			{
				var list = (sexp as SExpr.List).Items;
				if (list.IsEmpty)
					return new LiteralExpression (null);
				var symbol = list.First as SExpr.Symbol;
				if (symbol != null)
				{
					// Check if we have any of the special forms as first item.
					switch (symbol.Name)
					{
						case "quote":
							return new QuoteExpression (slist);
						case "if":
							return new IfExpression (slist);
						case "let":
							return new LetExpression (slist);
						case "lambda":
							return new LambdaExpression (slist);
						case "set!":
							return new SetExpression (slist);
					}
				}
				// Otherwise do a function call.
				return new ApplicationExpression (slist);
			}
			return new LiteralExpression (sexp as SExpr.Literal);
		}
	}
}