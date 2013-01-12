namespace NOP
{
	using System;
    using Collections;
    using Visuals;
    using System.Collections.Generic;

	public class IfExpression : Expression
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

        public void ChangeVisualGenerators()
        {
            var skeyword = ((SExpr.List)SExp).Items.First;
            var scond = Condition.SExp;
            var sthen = ThenExpression.SExp;
            var selse = ElseExpression.SExp;
        }
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			return TypeExpr.Builder.If (Condition.GetTypeExpr (), ThenExpression.GetTypeExpr (),
			                            ElseExpression.GetTypeExpr ());
		}

        protected override IEnumerable<AstNode> GetChildNodes ()
        {
            return List.Create (Condition, ThenExpression, ElseExpression);
        }
	}
}