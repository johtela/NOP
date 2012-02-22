namespace NOP
{
	using System;
	using Collections;

	public class SList : SExpr
	{
		public readonly List<SExpr> Items;
		
		public SList (List<SExpr> items)
		{
			Items = items;
		}
	}
}

