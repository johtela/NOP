namespace NOP
{
	using System;
	using NOP.Collections;
    using System.Collections.Generic;

	public class GenericTypeReference : TypeReference
	{
		public readonly SymbolExpression TypeName;
		public readonly NOPList<TypeReference> TypeParams;

		public GenericTypeReference (SExpr.List typeExpr) : base (typeExpr)
		{
			TypeName = new SymbolExpression ((SExpr.Symbol)typeExpr.Items.First);
			TypeParams = typeExpr.Items.Rest.Map (sexp => ParseTypeExpression (sexp));
		}

        protected override IEnumerable<AstNode> GetChildNodes ()
        {
            return TypeParams.Prepend<AstNode> (TypeName);
        }
	}
}