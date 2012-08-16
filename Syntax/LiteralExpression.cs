namespace NOP
{
	using System;

	public class LiteralExpression : AtomExpression
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
	}
}