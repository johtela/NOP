namespace NOP
{
	using System;

	public class LiteralExpression : Expression
	{
		public readonly SExpr.Literal Literal;
		
		public LiteralExpression (SExpr.Literal literal) : base (literal)
		{
			Literal = literal;
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			return TypeExpr.Builder.Lit (Literal.Value);
		}

		protected override Visual GetVisual ()
		{
			return Literal.Value is string ?
				SExp.Depiction = Visual.Label (string.Format ("\"{0}\"", Literal.Value)) :
				base.GetVisual ();
		}
	}
}