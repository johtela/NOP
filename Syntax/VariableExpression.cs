namespace NOP
{
	using System;

	public class VariableExpression : AtomExpression
	{
		public readonly Variable Variable;
		
		public VariableExpression (Variable variable) : base (variable)
		{
			Variable = variable;
		}
	}
}

