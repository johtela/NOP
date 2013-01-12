namespace NOP
{
	using System;
	using Collections;
    using System.Collections.Generic;

	public class LambdaExpression : Expression
	{
		public readonly NOPList<SymbolExpression> Parameters;
		public readonly Expression FunctionBody;
		
		public LambdaExpression (SExpr.List lambdaExpr) : base (lambdaExpr)
		{
			var sexps = lambdaExpr.Items.Rest;
			var pars = Expect<SExpr.List> (ref sexps, "list of parameters");
			Parameters = pars.Items.Map (sexp =>
			{
				var par = sexp as SExpr.Symbol;
				if (par == null)
					ParseError (sexp, "Expected a symbol");
				return new SymbolExpression (par);
			}
			);
			if (sexps.IsEmpty)
				ParseError (lambdaExpr, "Function body is missing");
			FunctionBody = Parse (sexps.First);
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			return TypeExpr.Builder.MultiLam (Parameters.Map (s => s.Symbol.Name), 
				FunctionBody.GetTypeExpr ());
		}

        protected override IEnumerable<AstNode> GetChildNodes ()
        {
            return Parameters.Append<AstNode> (FunctionBody);
        }
	}
}

