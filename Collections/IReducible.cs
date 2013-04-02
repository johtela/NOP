namespace NOP.Collections
{
	using System;

	/// <summary>
	/// Interface for any structure that can be reduced to a single value.
	/// </summary>
	public interface IReducible<T>
	{
		U ReduceLeft<U> (U acc, Func<U, T, U> func);
		U ReduceRight<U> (Func<T, U, U> func, U acc);
	}
}
