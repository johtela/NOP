namespace NOP
{
	using System;

	/// <summary>
	/// Interface for any structure that can be reduced to a single value starting 
	/// from a leftmost item.
	/// </summary>
	public interface ILeftReducible<T>
	{
		U ReduceLeft<U> (U acc, Func<U, T, U> func);
	}

	/// <summary>
	/// Interface for any structure that can be reduced to a single value starting
	/// from a rightmost item.
	/// </summary>
	public interface IRightReducible<T>
	{
		U ReduceRight<U> (Func<T, U, U> func, U acc);
	}

	/// <summary>
	/// Interface for any structure that can be reduced to a single value from
	/// both directions.
	/// </summary>
	public interface IReducible<T> : ILeftReducible<T>, IRightReducible<T> { }

	/// <summary>
	/// Split divides a reducible structure into parts according to a predicate.
	/// The split consists of three parts:
	/// - the left part is a reducible structure that contains all the items for whích 
	///   the predicate is not true
	/// - the first item for which the precicate is true
	/// - the right part is a reducible structure that contains the rest of the items 
	///   for which the predicate is true
	/// The left and right parts are lazy.
	/// </summary>
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
	/// Extension  methods for reducibles.
	/// </summary>
	public static class Reducible
	{
		private class _LeftRecurse<T> : ILeftReducible<T> where T : ILeftReducible<T>
		{
			private ILeftReducible<T> _reducible;

			public _LeftRecurse (ILeftReducible<T> reducible)
			{
				_reducible = reducible;
			}

			public U ReduceLeft<U> (U acc, Func<U, T, U> func)
			{
				return _reducible.ReduceLeft (acc, (a, r) => r.ReduceLeft (a, func));
			}
		}

		private class _RightRecurse<T> : IRightReducible<T> where T : IRightReducible<T>
		{
			private IRightReducible<T> _reducible;

			public _RightRecurse (IRightReducible<T> reducible)
			{
				_reducible = reducible;
			}

			public U ReduceRight<U> (Func<T, U, U> func, U acc)
			{
				return _reducible.ReduceRight ((r, a) => r.ReduceRight (func, a), acc);
			}
		}

		private class _LeftMap<T, V> : ILeftReducible<V>
		{
			private ILeftReducible<T> _reducible;
			private Func<T, V> _map;

			public _LeftMap (ILeftReducible<T> reducible, Func<T, V> map)
			{
				_reducible = reducible;
				_map = map;
			}

			public U ReduceLeft<U> (U acc, Func<U, V, U> func)
			{
				return _reducible.ReduceLeft (acc, (a, t) => func (a, _map(t)));
			}
		}

		private class _RightMap<T, V> : IRightReducible<V>
		{
			private IRightReducible<T> _reducible;
			private Func<T, V> _map;

			public _RightMap (IRightReducible<T> reducible, Func<T, V> map)
			{
				_reducible = reducible;
				_map = map;
			}

			public U ReduceRight<U> (Func<V, U, U> func, U acc)
			{
				return _reducible.ReduceRight ((t, a) => func (_map (t), a), acc);
			}
		}

		private class _LeftConcat<T> : ILeftReducible<T>
		{
			public readonly ILeftReducible<T> First;
			public readonly ILeftReducible<T> Second;

			public _LeftConcat (ILeftReducible<T> first, ILeftReducible<T> second)
			{
				First = first;
				Second = second;
			}

			public U ReduceLeft<U> (U acc, Func<U, T, U> func)
			{
				return Second.ReduceLeft (First.ReduceLeft (acc, func), func);
			}
		}

		private class _RightConcat<T> : IRightReducible<T>
		{
			public readonly IRightReducible<T> First;
			public readonly IRightReducible<T> Second;

			public _RightConcat (IRightReducible<T> first, IRightReducible<T> second)
			{
				First = first;
				Second = second;
			}

			public U ReduceRight<U> (Func<T, U, U> func, U acc)
			{
				return First.ReduceRight (func, Second.ReduceRight (func, acc));
			}
		}

		public static ILeftReducible<T> LeftRecurse<T> (this ILeftReducible<T> reducible) where T : ILeftReducible<T>
		{
			return new _LeftRecurse<T> (reducible);
		}

		public static IRightReducible<T> RightRecurse<T> (this IRightReducible<T> reducible) where T : IRightReducible<T>
		{
			return new _RightRecurse<T> (reducible);
		}

		public static ILeftReducible<V> LeftMap<T, V> (this ILeftReducible<T> reducible, Func<T, V> map)
		{
			return new _LeftMap<T, V> (reducible, map);
		}

		public static IRightReducible<V> RightMap<T, V> (this IRightReducible<T> reducible, Func<T, V> map)
		{
			return new _RightMap<T, V> (reducible, map);
		}

		public static ILeftReducible<V> LeftCast<T, V> (this ILeftReducible<T> reducible) where T : V
		{
			return reducible.LeftMap (t => (V)t);
		}

		public static IRightReducible<V> RightCast<T, V> (this IRightReducible<T> reducible) where T : V
		{
			return reducible.RightMap (t => (V)t);
		}

		public static ILeftReducible<T> LeftConcat<T> (this ILeftReducible<T> first, ILeftReducible<T> second)
		{
			return new _LeftConcat<T> (first, second);
		}

		public static IRightReducible<T> RightConcat<T> (this IRightReducible<T> first, IRightReducible<T> second)
		{
			return new _RightConcat<T> (first, second);
		}

		public static void Foreach<T> (this ILeftReducible<T> reducible, Action<T> action)
		{
			reducible.ReduceLeft<object> (null, (_, item) =>
			{
				action (item);
				return null;
			});
		}

		public static void Foreach<T> (this ILeftReducible<T> reducible, int initial, Action<T, int> action)
		{
			Foreach (reducible, initial, 1, action);
		}

		public static void Foreach<T> (this ILeftReducible<T> reducible, int initial, int increment, 
			Action<T, int> action)
		{
			reducible.ReduceLeft (initial, (i, item) =>
			{
				action (item, i);
				return i + increment;
			});
		}

		public static bool IterateWhile<T> (this ILeftReducible<T> reducible, int initial, Func<T, int, bool> func)
		{
			return reducible.ReduceLeft (Tuple.Create (initial, true), (res, item) =>
				res.Item2 ?
					Tuple.Create (res.Item1 + 1, func (item, res.Item1)) :
					Tuple.Create (0, false)
				).Item2;
		}
	}
}