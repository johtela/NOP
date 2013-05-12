namespace NOP
{
	using System;
	using System.Collections.Generic;
	using Collections;
	using NOP.Visuals;
	using V = NOP.Visuals.Visual;
	
	public class LetExpression : Expression
	{
		public readonly SymbolExpression Variable;
		public readonly Expression Value;
		public readonly Expression Body;

		public LetExpression (SExpr sexp, SymbolExpression variable, Expression value,
			Expression body) : base (sexp)
		{
			Variable = variable;
			Value = value;
			Body = body;
		}

		public LetExpression (SExpr.List letExpr) : base (letExpr)
		{
			var sexps = letExpr.Items.RestL;
			Variable = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "variable"));
			Value = Parse (Expect<SExpr> (ref sexps, "variable value"));
			Body = Parse (Expect<SExpr> (ref sexps, "body of let expression"));		
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			return TypeExpr.Builder.Let (Variable.Symbol.Name, Value.GetTypeExpr (), 
										 Body.GetTypeExpr ());
		}

		protected override void DoForChildNodes (Action<AstNode> action)
		{
			action (Variable);
			action (Value);
			action (Body);
		}

		protected override Visual GetVisual ()
		{
			var slet = ((SExpr.List)SExp).Items.First;
			var svar = Variable.SExp;
			var sval = Value.SExp;
			var sbody = Body.SExp;

			return V.VStack (HAlign.Left,
				V.HStack (VAlign.Top, V.Depiction (slet), V.Depiction (svar), 
					V.Label ("="), V.Depiction (sval)),
				V.HStack (VAlign.Top, V.Depiction (sbody)));
		}
	}
}