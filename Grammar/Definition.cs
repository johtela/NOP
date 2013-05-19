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

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (Name);
				Members.Foreach (action);
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

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (Variable);
				action (Value);
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