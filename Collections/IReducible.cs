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

	/// <summary>
	/// Extension  methods for IReducible interface. />
	/// </summary>
	public static class Reducible
	{
		public static void Foreach<T> (this IReducible<T> reducible, Action<T> action)
		{
			reducible.ReduceLeft<object> (null, (_, item) =>
			{
				action (item);
				return null;
			});
		}

		public static void Foreach<T> (this IReducible<T> reducible, int initial, Action<T, int> action)
		{
			Foreach (reducible, initial, 1, action);
		}

		public static void Foreach<T> (this IReducible<T> reducible, int initial, int increment, 
			Action<T, int> action)
		{
			reducible.ReduceLeft (initial, (i, item) =>
			{
				action (item, i);
				return i + increment;
			});
		}
	}
}
