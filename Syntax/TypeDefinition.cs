namespace NOP
{
	using System;
	using NOP.Collections;
	using System.Collections.Generic;

	public class TypeDefinition : Definition
	{
		public readonly SymbolExpression Name;
		public readonly StrictList<Definition> Members;
		
		public TypeDefinition (SExpr.List typeDef) : base (typeDef)
		{
			var sexps = typeDef.Items.RestL;
			Name = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "type name"));
			Members = List.MapReducible (sexps, sexp => Parse (sexp));
		}

		protected override void DoForChildNodes (Action<AstNode> action)
		{
			action (Name);
			Members.Foreach (action);
		}
	}
}

