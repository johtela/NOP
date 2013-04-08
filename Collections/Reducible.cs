namespace NOP.Collections
{
	using System;

	/// <summary>
	/// Interface for any data structure that can be reduced to a single value.
	/// </summary>
	public interface IReducible<T>
	{
		U ReduceLeft<U> (U acc, Func<U, T, U> func);
		U ReduceRight<U> (Func<T, U, U> func, U acc);
	}

	/// <summary>
	/// Split divides a reducible structure into parts according to a predicate.
	/// </summary>
	/// <remarks>
	/// The split consists of three parts:
	/// <list type="bullet">
	/// <item>the left part is a reducible structure that contains all the items for whích 
	///   the predicate is not true</item>
	/// <item>the first item for which the precicate is true</item>
	/// <item>the right part is a reducible structure that contains the rest of the items 
	///   for which the predicate is true</item>
	/// </list>
	/// The left and right parts are lazy.
	/// </remarks>
	public class Split<T, U, V>
		where T : IReducible<U>
		where U : IMeasurable<V>
		where V : IMonoid<V>, new ()
	{
		private readonly Lazy<T> _left;
		private readonly U _item;
		private readonly Lazy<T> _right;

		public Split (Lazy<T> left, U item, Lazy<T> right)
		{
			_left = left;
			_item = item;
			_right = right;
		}

		public T Left
		{
			get { return _left; }
		}

		public U Item
		{
			get { return _item; }
		}

		public T Right
		{
			get { return _right; }
		}
	}

	/// <summary>
	/// Interface for reducible data structures that can be splitted.
	/// </summary>
	public interface ISplittable<T, U, V>
		where T : IReducible<U>
		where U : IMeasurable<V>
		where V : IMonoid<V>, new ()
	{
		Split<T, U, V> Split (Func<V, bool> predicate, V acc);
	}

	/// <summary>
	/// Extension  methods for reducibles. />
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

		public static IReducible<T> Concat<T> (this IReducible<T> first, IReducible<T> second)
		{
			return new _Concat<T> (first, second);
		}

		private class _Concat<T> : IReducible<T>
		{
			public readonly IReducible<T> First;
			public readonly IReducible<T> Second;

			public _Concat (IReducible<T> first, IReducible<T> second)
			{
				First = first;
				Second = second;
			}

			public U ReduceLeft<U> (U acc, Func<U, T, U> func)
			{
				return Second.ReduceLeft (First.ReduceLeft (acc, func), func);
			}

			public U ReduceRight<U> (Func<T, U, U> func, U acc)
			{
				return First.ReduceRight (func, Second.ReduceRight (func, acc));
			}
		}
	}
}
