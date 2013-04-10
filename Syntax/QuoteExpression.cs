namespace NOP
{
	using System.Collections.Generic;
	using Collections;
	
	public class QuoteExpression : Expression
	{
		public readonly Expression QuotedExpression;
		
		public QuoteExpression (SExpr.List quoteSExp) : base (quoteSExp)
		{
			var sexps = quoteSExp.Items.RestL;
			QuotedExpression = Parse (Expect<SExpr> (ref sexps, "quoted expression"));
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			return TypeExpr.Builder.Lit (SExp);
		}

		protected override IEnumerable<AstNode> GetChildNodes ()
		{
			return List.Cons (QuotedExpression);
		}
	}
}

