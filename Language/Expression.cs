namespace NOP
{
	using System;
	using ExprList = NOP.Collections.List<object>;

	/// <summary>
	/// Delegate type for (static) functions.
	/// </summary>
	public delegate object Func (ExprList args);
	
	/// <summary>
	/// Delegate type for (dynamic) methods.
	/// </summary>
	public delegate object Meth (object obj, ExprList args);
	
	/// <summary>
	/// Result of the interpreter evaluation.
	/// </summary>
	public struct EvalResult
	{
		public readonly Environment Env;
		public readonly object Result;

		public EvalResult (Environment env, object result)
		{
			Env = env;
			Result = result;
		}
	}
	
	/// <summary>
	/// Parser state.
	/// </summary>
	public class ParserState<T>
	{
		public readonly T State;
		public readonly Func<Expression, T, ParserState<T>> NextHandler;
		
		public ParserState (T state, Func<Expression, T, ParserState<T>> nextHandler)
		{
			State = state;
			NextHandler = nextHandler;
		}
	}

	/// <summary>
	/// Abstract class representing any language expression.
	/// </summary>
	public abstract class Expression
	{
		/// <summary>
		/// Parsing traverses through the program structure calling a function for each
		/// expression parsed.
		/// </summary>
		public abstract T Parse<T> (ParserState<T> state);
	}
}