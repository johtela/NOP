namespace NOP
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Collections;
	
	/// <summary>
	/// Extension methods for .NET framework classes.
	/// </summary>
	public static class Extensions
	{
		#region Array extensions

		public static T[] Segment<T> (this T[] array, int first, int length)
		{
			if (first < 0 || first >= array.Length)
				throw new ArgumentException ("First is out of array index range", "first");
			if (length < 0 || (first + length) > array.Length)
				throw new ArgumentException ("Length is out of array index range", "length");
			var result = new T[length];
			Array.Copy (array, first, result, 0, length);
			return result;
		}

		public static U ReduceLeft<T, U> (this T[] array, U acc, Func<U, T, U> func)
		{
			for (int i = 0; i < array.Length; i++)
				acc = func (acc, array[i]);
			return acc;
		}

		public static U ReduceRight<T, U> (this T[] array, Func<T, U, U> func, U acc)
		{
			for (int i = array.Length - 1; i >= 0; i--)
				acc = func (array[i], acc);
			return acc;
		}

		#endregion

		#region IEnumerable extensions

		public static IEnumerable<T> Append<T> (this IEnumerable<T> enumerable, T item)
		{
			foreach (T i in enumerable)
				yield return i;
			yield return item;
		}

		public static IEnumerable<T> Prepend<T> (this IEnumerable<T> enumerable, T item)
		{
			yield return item;
			foreach (T i in enumerable)
				yield return i;
		}

		public static IEnumerable<T> Loop<T> (this IEnumerable<T> enumerable)
		{
			while (true)
				foreach (T i in enumerable)
					yield return i;
		}

		public static IEnumerable<V> Combine<T, U, V> (this IEnumerable<T> enum1, 
			IEnumerable<U> enum2, Func<T, U, V> combine)
		{
			var e1 = enum1.GetEnumerator ();
			var e2 = enum2.GetEnumerator ();
			var i1 = default (T);
			var i2 = default (U);

			while (true)
			{
				var b1 = e1.MoveNext();
				var b2 = e2.MoveNext();

				if (!b1 && !b2) break;
				if (b1)	i1 = e1.Current;
				if (b2)	i2 = e2.Current;
				yield return combine (i1, i2);
			}
		}

		public static void ForEach<T> (this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var x in enumerable)
				action (x);
		}

		#endregion

		#region Tuple extensions
				
		public static void Bind<T, U> (this Tuple<T, U> tuple, Action<T, U> action)
		{
			action (tuple.Item1, tuple.Item2);
		}

		public static V Bind<T, U, V> (this Tuple<T, U> tuple, Func<T, U, V> func)
		{
			return func (tuple.Item1, tuple.Item2);
		}

		public static bool Match<T, U> (this Tuple<T, U> tuple, T first, out U second)
		{
			if (tuple.Item1.Equals (first))
			{
				second = tuple.Item2;
				return true;
			}
			else
			{
				second = default (U);
				return false;
			}
		}

		public static bool Match<T, U> (this Tuple<T, U> tuple, out T first, U second)
		{
			if (tuple.Item2.Equals (second))
			{
				first = tuple.Item1;
				return true;
			}
			else
			{
				first = default (T);
				return false;
			}
		}

		public static bool Match<T, U> (this Tuple<T, U> tuple, Func<T, bool> predicate, out U second)
		{
			if (predicate (tuple.Item1))
			{
				second = tuple.Item2;
				return true;
			}
			else
			{
				second = default (U);
				return false;
			}
		}

		#endregion    

		#region String extensions
			   
		public static string Times (this string what, int times)
		{
			var sb = new StringBuilder ();

			for (int i = 0; i < times; i++)
				sb.Append (what);

			return sb.ToString ();
		}

		#endregion

		#region SExpr extensions

		public static Sequence<SExpr> AsSequence (this SExpr sexp)
		{
			return (sexp as SExpr.List).Items;
		}

		#endregion
	}
}