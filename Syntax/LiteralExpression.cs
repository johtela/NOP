namespace NOP
{
	using System;
	using Visuals;

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
				Visual.Label (string.Format ("\"{0}\"", Literal.Value)) :
				base.GetVisual ();
		}
	}
}