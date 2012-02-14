namespace NOP
{
	using System;
	using Collections;

	public class ListExpression : Expression
	{
		public readonly List<Expression> Items;
		
		public ListExpression (List<Expression> items)
		{
			Items = items;
		}
		
		public override T Parse<T> (ParserState<T> state)
		{
			throw new NotImplementedException ();
		}
	}
}

