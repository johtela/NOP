namespace NOP
{
	using System;
    using System.Collections.Generic;
    using NOP.Collections;

	public class BasicTypeReference : TypeReference
	{
		public readonly SymbolExpression TypeName;

		public BasicTypeReference (SExpr.Symbol typeName) : base (typeName)
		{
			TypeName = new SymbolExpression (typeName);
		}

        protected override IEnumerable<AstNode> GetChildNodes ()
        {
            return List.Cons (TypeName);
        }
	}
}

