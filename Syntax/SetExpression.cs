namespace NOP
{
	using System;

	public class SetExpression : ListExpression
	{
		public readonly SymbolExpression Lhs;
		public readonly Expression Rhs;
		
		public SetExpression (SList setExpr) : base (setExpr)
		{
			var sexps = setExpr.Items.Rest;
			Lhs = new SymbolExpression (Expect<Symbol> (ref sexps, "variable"));
			Rhs = Parse (Expect<SExpr> (ref sexps, "right hand side of set! clause"));			
		}
	}
}