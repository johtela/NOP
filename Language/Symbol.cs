namespace NOP
{
	using System;
	
	/// <summary>
	/// Symbols used by the programs. 
	/// </summary>
	public sealed class Symbol : Expression
	{
		public readonly string Name;

		public Symbol (string name)
		{
			Name = name;
		}

		public override bool Equals (object obj)
		{
			return (obj is Symbol) && Name.Equals (obj);
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode ();
		}

		public override string ToString ()
		{
			return Name;
		}
		
		public override T Parse<T> (ParserState<T> state)
		{
			return state.NextHandler (this, state.State).State;
		}
		
		protected override ParserState<EvalResult> Evaluate (EvalResult lastResult)
		{
			return new ParserState<EvalResult> (new EvalResult (lastResult.Env, lastResult.Env.Lookup (Name)), null);
		}
	}
}