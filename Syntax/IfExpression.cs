namespace NOP
{
	using System;

	public class IfExpression : ListExpression
	{
		public readonly Expression Condition;
		public readonly Expression ThenExpression;
		public readonly Expression ElseExpression;
		
		public IfExpression (SExpr.List ifExpr) : base (ifExpr)
		{
			var sexps = ifExpr.Items.Rest;
			Condition = Parse (Expect<SExpr> (ref sexps, "condition"));
			ThenExpression = Parse (Expect<SExpr> (ref sexps, "then expression"));
			ElseExpression = Parse (Expect<SExpr> (ref sexps, "else expression"));
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			return TypeExpr.Builder.If (Condition.GetTypeExpr (), ThenExpression.GetTypeExpr (),
			                            ElseExpression.GetTypeExpr ());
		}
	}
}