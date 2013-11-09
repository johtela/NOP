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

		public override U ReduceLeft<U> (U acc, Func<U, AstNode, U> func)
		{
			return func (Value.ReduceLeft (Variable.ReduceLeft (acc, func), func), this);
		}

		public override U ReduceRight<U> (Func<AstNode, U, U> func, U acc)
		{
			return Value.ReduceRight (func, Variable.ReduceRight (func, func (this, acc)));
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