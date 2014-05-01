namespace NOP.Grammar
{
	using System;
	using NOP.Collections;
	using Visuals;
	using V = NOP.Visuals.Visual;

	public class Definition : AstNode
	{
		public readonly VariableDefinition Variable;
		public readonly Expression Value;

		internal Definition (SExpr sexp, VariableDefinition variable, Expression value)
			: base (sexp)
		{
			Variable = variable;
			Value = value;
		}

		public override void VisitNodes (Action<AstNode> visitor)
		{
			Variable.VisitNodes (visitor);
			Value.VisitNodes (visitor);
			base.VisitNodes (visitor);
		}

		protected override Visuals.Visual GetVisual ()
		{
			var sexps = ((SExpr.List)SExp).Items;
			var def = sexps.First;
			var variable = sexps.RestL.First;
			var value = sexps.RestL.RestL.First;

			return V.HStack (VAlign.Top, V.Depiction (def), 
				V.Depiction (variable), V.Label ("="), V.Depiction (value));
		}
	}
}