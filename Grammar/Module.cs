namespace NOP.Grammar
{
	using System;
	using NOP.Collections;

	public class Module : AstNode
	{
		public readonly Expression._Symbol Name;
		public readonly StrictList<Definition> Members;

		internal Module (SExpr sexp, Expression._Symbol name, StrictList<Definition> members)
			: base (sexp)
		{
			Name = name;
			Members = members;
		}

		public override U ReduceLeft<U> (U acc, Func<U, AstNode, U> func)
		{
			return func (Members.ReduceLeft (Name.ReduceLeft (acc, func), func), this);
		}

		public override U ReduceRight<U> (Func<AstNode, U, U> func, U acc)
		{
			return Members.ReduceRight (func, Name.ReduceRight (func, func (this, acc)));
		}
	}
}
