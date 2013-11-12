namespace NOP.Grammar
{
	using System;
	using Collections;
	using System.Collections.Generic;
	using Visuals;
	using CodeGen;

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

	public abstract class AstNode : ILeftReducible<AstNode>
	{
		public readonly SExpr SExp;
		
		public AstNode (SExpr sexp)
		{
			SExp = sexp;
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
		protected T Expect<T> (ref Sequence<SExpr> sexps, string token) where T: SExpr
		{
			if (sexps.IsEmpty)
				ParseError (SExp, string.Format ("Expected {0} but reached the end of list.", 
					token)
				);
			var sexp = sexps.First as T;
			if (sexp == null)
				ParseError (sexps.First, string.Format ("Expected {0} but got {1}.", 
					token, sexps.First)
				);
			sexps = sexps.RestL;
			return sexp;
		}
		
		/// <summary>
		/// Checks if the next S-expression in the list is of given type. 
		/// Raises an error if the list is exhausted. If the specified S-expression is 
		/// read the list is advanced to the next item. Otherwise the list remains the same.
		/// </summary>
		protected bool NextSExp<T> (ref Sequence<SExpr> sexps, out T sexp) where T: SExpr
		{
			if (sexps.IsEmpty)
				ParseError (SExp, string.Format ("Unexpected end of list."));
			sexp = sexps.First as T;
			if (sexp != null)
			{
				sexps = sexps.RestL;
				return true;
			}
			return false;
		}
	
		/// <summary>
		/// Change the visual depiction of the AST node.
		/// </summary>
		protected virtual Visual GetVisual ()
		{
			return SExp.Depiction;
		}

		public void ChangeVisualDepictions ()
		{
			this.Foreach (node => node.SExp.Depiction = node.GetVisual ());
		}

		protected virtual ILeftReducible<AstNode> AsReducible ()
		{
			return null;
		}

		#region IReducible<AstNode> implementation
		
		public U ReduceLeft<U> (U acc, Func<U, AstNode, U> func)
		{
			var reducible = AsReducible ();
			return reducible != null ?
				func (reducible.ReduceLeft (acc, func), this) :
				func (acc, this);
		}

		#endregion	
	}
}

