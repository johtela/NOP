namespace NOP
{
	using System;

	public abstract class AtomExpression : Expression
	{
		public AtomExpression (SExpr atom) : base(atom)
		{
		}
	}
}

