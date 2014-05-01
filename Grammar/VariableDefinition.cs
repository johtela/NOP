namespace NOP.Grammar
{
	using System;
	using System.Collections.Generic;
	using NOP.Collections;
	using Visuals;
	using V = NOP.Visuals.Visual;

	public class VariableDefinition : AstNode
	{
		public readonly Expression._Symbol Name;
		public readonly TypeReference Type;

		public VariableDefinition (SExpr sexp, Expression._Symbol name, TypeReference type)
			: base (sexp)
		{
			Name = name;
			Type = type;
		}

		public override void VisitNodes (Action<AstNode> visitor)
		{
			Name.VisitNodes (visitor);
			if (Type != null)
				Type.VisitNodes (visitor);
			base.VisitNodes (visitor);
		}

		protected override V GetVisual ()
		{
			if (SExp is SExpr.List)
			{
				var sexps = ((SExpr.List)SExp).Items;
				var variable = sexps.First;
				var vartype = sexps.RestL.First;

				return V.HStack (VAlign.Top, V.Depiction (variable), V.Label (":"), V.Depiction (vartype));
			}
			else return base.GetVisual ();
		}
	}
}