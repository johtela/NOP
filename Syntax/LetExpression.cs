namespace NOP
{
	using System;
    using System.Collections.Generic;
    using Collections;
	
	public class LetExpression : Expression
	{
		public readonly SymbolExpression Variable;
		public readonly Expression Value;
		public readonly Expression Body;
		
		public LetExpression (SExpr.List letExpr) : base (letExpr)
		{
			var sexps = letExpr.Items.Rest;
			Variable = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "variable"));
			Value = Parse (Expect<SExpr> (ref sexps, "variable value"));
			Body = Parse (Expect<SExpr> (ref sexps, "body of let expression"));		
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			return TypeExpr.Builder.Let (Variable.Symbol.Name, Value.GetTypeExpr (), 
			                             Body.GetTypeExpr ());
		}

        protected override IEnumerable<AstNode> GetChildNodes ()
        {
            return List.Create<AstNode> (Variable, Value, Body);
        }
	}
}