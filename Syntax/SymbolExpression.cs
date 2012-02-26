namespace NOP
{
	using System;

	public class SymbolExpression : AtomExpression
	{
		public readonly Symbol Symbol;
		
		public SymbolExpression (Symbol symbol) : base (symbol)
		{
			Symbol = symbol;
		}
	}
}

