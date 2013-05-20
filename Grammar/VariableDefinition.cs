namespace NOP.Grammar
{
	using System;
	using System.Collections.Generic;
	using NOP.Collections;

	public class VariableDefinition : AstNode
	{
		public readonly Expression._Symbol Name;
		public readonly TypeReference Type;

		public VariableDefinition (SExpr sexp, Expression._Symbol name, TypeReference type)
			: base (sexp)
		{
			Name = name;
			Type = type;
		}

		protected override void DoForChildNodes (Action<AstNode> action)
		{
			action (Name);
			if (Type != null) action (Type);
		}
	}
}