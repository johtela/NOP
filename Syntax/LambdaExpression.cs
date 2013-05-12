namespace NOP
{
	using System;
	using Collections;
	using System.Collections.Generic;
	using Visuals;
	using V = NOP.Visuals.Visual;

	public class LambdaExpression : Expression
	{
		public readonly StrictList<SymbolExpression> Parameters;
		public readonly Expression FunctionBody;

		public LambdaExpression (SExpr sexp, StrictList<SymbolExpression> parameters,
			Expression functionBody) : base (sexp)
		{
			Parameters = parameters;
			FunctionBody = functionBody;
		}

		public LambdaExpression (SExpr.List lambdaExpr) : base (lambdaExpr)
		{
			var sexps = lambdaExpr.Items.RestL;
			var pars = Expect<SExpr.List> (ref sexps, "list of parameters");
			Parameters = List.MapReducible (pars.Items, sexp =>
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
			return TypeExpr.Builder.MultiLam (Parameters.Map (s => s.Symbol.Name), 
				FunctionBody.GetTypeExpr ());
		}

		protected override void DoForChildNodes (Action<AstNode> action)
		{
			Parameters.Foreach (action);
			action (FunctionBody);
		}

		protected override Visual GetVisual ()
		{
			var sexps = ((SExpr.List)SExp).Items;
			var slambda = sexps.First;
			var sparams = sexps.RestL.First;
			var sbody = sexps.RestL.RestL.First;

			return V.VStack (HAlign.Left,
				V.HStack (VAlign.Top, V.Frame (V.Depiction (slambda), FrameKind.Rectangle), V.Depiction (sparams)),
				V.Margin (V.Depiction (sbody), 24));
		}
	}
}