namespace NOP
{
	using System;
	using NOP.Collections;
    using System.Collections.Generic;

	public class TypeDefinition : Definition
	{
		public readonly SymbolExpression Name;
		public readonly NOPList<Definition> Members;
		
		public TypeDefinition (SExpr.List typeDef) : base (typeDef)
		{
			var sexps = typeDef.Items.Rest;
			Name = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "type name"));
			Members = sexps.Map (sexp => Parse (sexp));
		}

        protected override IEnumerable<AstNode> GetChildNodes ()
        {
            return Members.Prepend<AstNode> (Name);
        }
	}
}

