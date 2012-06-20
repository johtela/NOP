namespace NOP
{
	using System;

	public class SetExpression : ListExpression
	{
		public readonly SymbolExpression Lhs;
		public readonly Expression Rhs;
		
		public SetExpression (SExpr.List setExpr) : base (setExpr)
		{
			var sexps = setExpr.Items.Rest;
			Lhs = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "variable"));
			Rhs = Parse (Expect<SExpr> (ref sexps, "right hand side of set! clause"));			
		}
	}
}