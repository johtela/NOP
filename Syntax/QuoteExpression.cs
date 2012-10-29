namespace NOP
{
	public class QuoteExpression : Expression
	{
		public readonly Expression QuotedExpression;
		
		public QuoteExpression (SExpr.List quoteSExp) : base (quoteSExp)
		{
			var sexps = quoteSExp.Items.Rest;
			QuotedExpression = Parse (Expect<SExpr> (ref sexps, "quoted expression"));
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			return TypeExpr.Builder.Lit (SExp);
		}
	}
}

