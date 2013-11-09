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

		public override U ReduceLeft<U> (U acc, Func<U, AstNode, U> func)
		{
			return Type != null ?
				func (Type.ReduceLeft (Name.ReduceLeft (acc, func), func), this) :
				func (Name.ReduceLeft (acc, func), this);
		}

		public override U ReduceRight<U> (Func<AstNode, U, U> func, U acc)
		{
			return Type != null ?
				Type.ReduceRight (func, Name.ReduceRight (func, func (this, acc))) :
				Name.ReduceRight (func, func (this, acc));
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