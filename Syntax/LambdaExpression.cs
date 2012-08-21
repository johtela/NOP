namespace NOP
{
	using System;
	using Collections;

	public class LambdaExpression : ListExpression
	{
		public readonly List<SymbolExpression> Parameters;
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
			});
			if (sexps.IsEmpty)
				ParseError (lambdaExpr, "Function body is missing");
			FunctionBody = Parse (sexps.First);
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			// TODO: Implement and remove the option to have variable length parameters.
			throw new NotImplementedException ();
		}
	}
}

