namespace NOP
{
	using System;
	using System.Collections.Generic;
	using NOP.Collections;

	public class SetExpression : Expression
	{
		public readonly SymbolExpression Variable;
		public readonly Expression Value;

		public SetExpression (SExpr sexp, SymbolExpression variable, Expression value)
			: base (sexp)
		{
			Variable = variable;
			Value = value;
		}
		
		public SetExpression (SExpr.List setExpr) : base (setExpr)
		{
			var sexps = setExpr.Items.RestL;
			Variable = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "variable"));
			Value = Parse (Expect<SExpr> (ref sexps, "right hand side of set! clause"));			
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			return TypeExpr.Builder.App (
				TypeExpr.Builder.App (TypeExpr.Builder.Var ("set!"), Variable.GetTypeExpr ()), 
				Value.GetTypeExpr ());
		}

		protected override void DoForChildNodes (Action<AstNode> action)
		{
			action (Variable);
			action (Value);
		}
	}
}