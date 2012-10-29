namespace NOP
{
	using System;

	public class SymbolExpression : Expression
	{
		public readonly SExpr.Symbol Symbol;
		
		public SymbolExpression (SExpr.Symbol symbol) : base (symbol)
		{
			Symbol = symbol;
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			return TypeExpr.Builder.Var (Symbol.Name);
		}
	}
}

