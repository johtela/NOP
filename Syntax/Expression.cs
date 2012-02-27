namespace NOP
{
	using System;
	using Collections;

	/// <summary>
	/// Exception for interpreter errors.
	/// </summary>
	public class ParserException : Exception
	{
		public readonly SExpr SExp;

		public ParserException (SExpr sexp, string message) : base(message)
		{
			SExp = sexp;
		}
	}

	/// <summary>
	/// Abstract class representing any language expression. The root class of the abstract
	/// syntax tree.
	/// </summary>
	public abstract class Expression
	{
		public readonly SExpr SExp;
		
		public Expression (SExpr sexp)
		{
			SExp = sexp;
		}
		
		/// <summary>
		/// Parse an S-expression and generate the AST.
		/// </summary>
		public static Expression Parse (SExpr sexp)
		{
			// Is this a symbol?
			if (sexp is Symbol)
				return new SymbolExpression (sexp as Symbol);
			// Or is it a list?
			var slist = sexp as SList;
			if (slist != null)
			{
				var list = (sexp as SList).Items;
				if (list.IsEmpty)
					return new LiteralExpression (null);
				var symbol = list.First as Symbol;
				if (symbol != null)
				{
					// Check if we have any of the special forms as first item.
					switch (symbol.Name)
					{
						case "quote":
							return new QuoteExpression (slist);
						case "if":
							return new IfExpression (slist);
						case "begin":
							return new BeginExpression (slist);
						case "define":
							return new DefineExpression (slist);
						case "lambda":
							return new LambdaExpression(slist);
//						case "set!":
//							return EvalSet (env, list.Rest);
//						default:
//							return InvokeFunction (env, symbol, list.Rest);
					}
				}
//				// Is this an external function call?
//				var func = list.First as Function;
//				if (func != null)
//					return InvokeFunction (env, func, list.Rest);
//				// Or is it a method call or property read?
//				var obj = list.First;
//				var rest = list.Rest;
//				if (!rest.IsEmpty)
//				{
//					if (rest.First is Method)
//						return InvokeMethod (env, obj, rest.First as Method, rest.Rest);
//					else
//					if (rest.First is Property)
//						return GetProperty (env, obj, rest.First as Property);
//				}
//				// Otherwise throw an error.
//				Error (rest.First, "Expected a function or method call, or property read.");
			}
//			var val = expr as Value;
//			if (val != null)
//				return new EvalResult (env, val.Get ());
			return new LiteralExpression (sexp as Literal);
		}

		protected static void ParseError (SExpr sexp, string message)
		{
			throw new ParserException (sexp, message);
		}
		
		/// <summary>
		/// A helper function to get the next token in the list of S-expressions. If the list 
		/// is exhausted or the type of the token does not match, an error is raised. Advances 
		/// the list of S-expressions to the next item.
		/// </summary>
		protected T Expect<T>(ref List<SExpr> sexps, string token) where T: SExpr
		{
			if (sexps.IsEmpty)
				ParseError (SExp, string.Format ("Expected {0} but reached the end of list.", 
					token));
			var sexp = sexps.First as T;
			if (sexp == null)
				ParseError (sexps.First, string.Format ("Expected {0} but got {1}.", 
					token, sexps.First));
			sexps = sexps.Rest;
			return sexp;
		}
		
		/// <summary>
		/// Checks if the next S-expression in the list is of given type. 
		/// Raises an error if the list is exhausted. If the specified S-expression is 
		/// read the list is advanced to the next item. Otherwise the list remains the same.
		/// </summary>
		protected bool NextSExp<T>(ref List<SExpr> sexps, out T sexp) where T: SExpr
		{
			if (sexps.IsEmpty)
				ParseError (SExp, string.Format ("Unexpected end of list."));
			sexp = sexps.First as T;
			if (sexp != null)
			{
				sexps = sexps.Rest;
				return true;
			}
			return false;
		}		
	}
}