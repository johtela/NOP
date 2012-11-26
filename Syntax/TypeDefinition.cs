namespace NOP
{
	using System;
	using NOP.Collections;

	public class TypeDefinition : Definition
	{
		public readonly SymbolExpression Name;
		public readonly List<Definition> Members;
		
		public TypeDefinition (SExpr.List typeDef) : base (typeDef)
		{
			var sexps = typeDef.Items.Rest;
			Name = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "type name"));
			Members = sexps.Map (sexp => Parse (sexp));
		}
	}
}

