namespace NOP.Grammar
{
	using System;
	using NOP.Collections;
	using Grammar;
	using Parsing;
	using Visuals;
	using V = NOP.Visuals.Visual;

	public class Module : AstNode
	{
		public readonly Expression._Symbol Name;
		public readonly StrictList<Definition> Members;

		internal Module (SExpr sexp, Expression._Symbol name, StrictList<Definition> members)
			: base (sexp)
		{
			Name = name;
			Members = members;
		}

		protected override ILeftReducible<AstNode> AsReducible ()
		{
			return Name.LeftConcat (Members.LeftCast<Definition, AstNode> ().LeftRecurse ());
		}

		protected override Visuals.Visual GetVisual ()
		{
			var sexps = ((SExpr.List)SExp).Items;
			var module = sexps.First;
			var name = sexps.RestL.First;
			var defs = sexps.RestL.RestL;

			return V.Indented(
				V.HStack (VAlign.Top, V.Depiction (module), V.Depiction (name)),
				V.VStack (HAlign.Left, defs.Map (V.Depiction)), 24);
		}

		/// <summary>
		/// Parse an S-expression and generate the AST.
		/// </summary>
		public static Module Parse (SExpr sexp)
		{
			return SExprParser.Module ().Parse (Input.FromSExpr (sexp));
		}
	}
}
