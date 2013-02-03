namespace NOP
{
	using System;
	using Collections;
	using System.Collections.Generic;
	using Visuals;

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

	public abstract class AstNode
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
		protected T Expect<T> (ref NOPList<SExpr> sexps, string token) where T: SExpr
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
			sexps = sexps.Rest;
			return sexp;
		}
		
		/// <summary>
		/// Checks if the next S-expression in the list is of given type. 
		/// Raises an error if the list is exhausted. If the specified S-expression is 
		/// read the list is advanced to the next item. Otherwise the list remains the same.
		/// </summary>
		protected bool NextSExp<T> (ref NOPList<SExpr> sexps, out T sexp) where T: SExpr
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
	
		/// <summary>
		/// Return the list of abstract syntax tree nodes under this node.
		/// Must be overridden by AST nodes that have children.
		/// </summary>
		protected virtual IEnumerable<AstNode> GetChildNodes ()
		{
			return NOPList<AstNode>.Empty;
		}

		/// <summary>
		/// Change the visual depiction of the AST node.
		/// </summary>
		protected virtual Visual GetVisual ()
		{
			return SExp.Depiction;
		}

		/// <summary>
		/// Walk through the AST tree depth first performing a given
		/// action for each node traversed.
		/// </summary>
		public void WalkTreeDepthFirst (Action<AstNode> action)
		{
			foreach (var child in GetChildNodes ())
				child.WalkTreeDepthFirst (action);
			action (this);
		}

		public void ChangeVisualDepictions ()
		{
			WalkTreeDepthFirst (node => node.SExp.Depiction = node.GetVisual ());
		}
	}
}

