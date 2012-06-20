namespace NOP
{
	public class QuoteExpression : ListExpression
	{
		public readonly Expression QuotedExpression;
		
		public QuoteExpression (SExpr.List quoteSExp) : base (quoteSExp)
		{
			var sexps = quoteSExp.Items.Rest;
			QuotedExpression = Parse (Expect<SExpr> (ref sexps, "quoted expression"));
		}
	}
}

