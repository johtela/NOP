namespace NOP.Grammar
{
	using System;
	using NOP.Collections;

	public abstract class Definition : AstNode
	{
		public Definition (SExpr sexp) : base (sexp)
		{
		}
		
		public class _Type : Definition
		{
			public readonly Expression._Symbol Name;
			public readonly StrictList<Definition> Members;

			internal _Type (SExpr sexp, Expression._Symbol name, StrictList<Definition> members)
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

		public class _Member : Definition
		{
			public readonly VariableDefinition Variable;
			public readonly Expression Value;

			internal _Member (SExpr sexp, VariableDefinition variable, Expression value)
				: base (sexp)
			{
				Variable = variable;
				Value = value;
			}

			public override U ReduceLeft<U> (U acc, Func<U, AstNode, U> func)
			{
				return func (Value.ReduceLeft (Variable.ReduceLeft (acc, func), func), this);
			}

			public override U ReduceRight<U> (Func<AstNode, U, U> func, U acc)
			{
				return Value.ReduceRight (func, Variable.ReduceRight (func, func (this, acc)));
			}
		}

		public static Definition Type (SExpr sexp, Expression._Symbol name, StrictList<Definition> members)
		{
			return new _Type (sexp, name, members);
		}

		public static Definition Member (SExpr sexp, VariableDefinition variable, Expression value)
		{
			return new _Member (sexp, variable, value);
		}
	}
}