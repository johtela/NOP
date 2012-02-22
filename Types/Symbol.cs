namespace NOP
{
	using System;

	public class Symbol : SExpr
	{
		public readonly string Name;
		
		public Symbol (string name)
		{
			Name = name;
		}
	}
}

