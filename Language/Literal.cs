namespace NOP
{
	using System;
	
	/// <summary>
	/// The atom is the base class for singular values in the language.
	/// </summary>
	public class Literal : Expression
	{
		public readonly object Value;
		
		public Literal (object value)
		{
			Value = value;
		}
		
		public override T Parse<T> (ParserState<T> state)
		{
			return state.NextHandler (this, state.State).State;
		}
	
		// Move to interpreter
		//protected override ParserState<EvalResult> Evaluate (EvalResult lastResult)
		//{
		//	return new ParserState<EvalResult>(new EvalResult (lastResult.Env, Value), null);
		//}
	}
}

