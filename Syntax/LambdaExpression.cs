namespace NOP
{
	using System;
	using Collections;

	public class LambdaExpression : ListExpression
	{
		public readonly List<SymbolExpression> Parameters;
		public readonly List<Expression> FunctionBody;
		
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
			var dot = pars.Items.FindNext (new SExpr.Symbol ("."));
			if (!(dot.IsEmpty || dot.Length == 2))
				ParseError (pars, "There should be only one parameter after '.'");
			if (sexps.IsEmpty)
				ParseError (lambdaExpr, "Function body is missing");
			FunctionBody = sexps.Map (sexp => Parse (sexp));
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			// TODO: Implement and remove the option to have variable length parameters.
			throw new NotImplementedException ();
		}
	}
}

