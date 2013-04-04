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
	/// Split divides a reducible structure into three parts according to a predicate: 
	/// - the left part is a reducible structure that contains all the items for whích 
	///   the predicate is not true
	/// - the first item for which the precicate is true
	/// - the right part is a reducible structure that contains the rest of the items 
	///   for which the predicate is true
	/// </summary>
	public class Split<T, U, V>
		where T : IReducible<U>
		where U : IMeasurable<V>
		where V : IMonoid<V>, new ()
	{
		public readonly T Left;
		public readonly U Item;
		public readonly T Right;

		public Split (T left, U item, T right)
		{
			Left = left;
			Item = item;
			Right = right;
		}
	}

	/// <summary>
	/// Interface for reducible structures that can be splitted.
	/// </summary>
	public interface ISplittable<T, U, V>
		where T : IReducible<U>
		where U : IMeasurable<V>
		where V : IMonoid<V>, new ()
	{
		Split<T, U, V> Split (Func<V, bool> predicate, V acc);
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
