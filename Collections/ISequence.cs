using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOP.Collections
{
	/// <summary>
	/// Immutable sequence that can be strict or lazy. A sequence
	/// has to implement also IFunctor to provide mapping support
	/// plus IReducible to provide folding functions.
	/// </summary>
	public interface ISequence<T> : IReducible<T>
	{
		/// <summary>
		/// The first element of the sequence.
		/// </summary>
		T First { get; }

		/// <summary>
		/// The rest of the sequence.
		/// </summary>
		ISequence<T> Rest { get; }

		/// <summary>
		/// Is the sequence empty.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Map over the sequence.
		/// </summary>
		ISequence<U> Map<U> (Func<T, U> map);

		/// <summary>
		/// Filter the sequence.
		/// </summary>
		ISequence<T> Filter (Func<T, bool> predicate);

		/// <summary>
		/// Collect items from set of sequences.
		/// </summary>
		/// <returns></returns>
		ISequence<U> Collect<U> (Func<T, ISequence<U>> func);
	}

	/// <summary>
	/// Extension methods for sequences.
	/// </summary>
	public static class SequenceExtensions
	{
		/// <summary>
		/// Count the length, i.e. the number of items, in the sequence.
		/// </summary>
		public static int Length<T> (this ISequence<T> seq)
		{
			int result = 0;

			while (!seq.IsEmpty)
			{
				result++;
				seq = seq.Rest;
			}
			return result;
		}

		/// <summary>
		/// Search for an item in the seqence.
		/// </summary>
		public static ISequence<T> FindNext<T> (this ISequence<T> seq, T item)
		{
			while (!seq.IsEmpty && !seq.First.Equals (item))
				seq = seq.Rest;
			return seq;
		}

		/// <summary>
		/// Find an item in the list that matches a predicate.
		/// </summary>
		public static ISequence<T> FindNext<T> (this ISequence<T> seq, Predicate<T> predicate)
		{
			while (!seq.IsEmpty && !predicate (seq.First))
				seq = seq.Rest;
			return seq;
		}

		/// <summary>
		/// Drop n items from the sequence.
		/// </summary>
		public static ISequence<T> Drop<T> (this ISequence<T> seq, int n)
		{
			while (n-- > 0)
				seq = seq.Rest;
			return seq;
		}

		/// <summary>
		/// Return the position of the specified item.
		/// </summary>
		public static int IndexOf<T> (this ISequence<T> seq, T item)
		{
			var i = 0;

			while (!seq.IsEmpty)
			{
				if (seq.First.Equals (item))
					return i;
				seq = seq.Rest;
				i++;
			}
			return -1;
		}

		/// <summary>
		/// Check if two lists are equal, that is contain the same items in
		/// the same order, and have equal lengths.
		/// </summary>
		public static bool EqualTo<T> (this ISequence<T> seq, ISequence<T> other, Func<T, T, bool> equals)
		{
			while (!seq.IsEmpty && !other.IsEmpty && equals (seq.First, other.First))
			{
				seq = seq.Rest;
				other = other.Rest;
			}
			return seq.IsEmpty && other.IsEmpty;
		}

		/// <summary>
		/// Check if two lists are equal, that is contain the same items in
		/// the same order, and have equal lengths.
		/// </summary>
		public static bool EqualTo<T> (this ISequence<T> seq, ISequence<T> other)
		{
			return seq.EqualTo (other, (i1, i2) => i1.Equals (i2));
		}

		/// <summary>
		/// Returns a string representing the list. Gets open, close bracket, and separtor as
		/// an argument.
		/// </summary>
		public static string ToString<T> (this ISequence<T> seq, string openBracket, string closeBracket, string separator)
		{
			StringBuilder sb = new StringBuilder (openBracket);

			while (!seq.IsEmpty)
			{
				sb.Append (seq.First);
				seq = seq.Rest;

				if (!seq.IsEmpty)
					sb.Append (separator);
			}
			sb.Append (closeBracket);
			return sb.ToString ();
		}

		/// <summary>
		/// Convert sequence to IEnumerable.
		/// </summary>
		public static IEnumerable<T> ToEnumerable<T> (this ISequence<T> seq)
		{
			while (!seq.IsEmpty)
			{
				yield return seq.First;
				seq = seq.Rest;
			}
		}

		public static ISequence<U> Select<T, U> (this ISequence<T> seq, Func<T, U> select)
		{
			return seq.Map (select);
		}
		
		public static ISequence<V> SelectMany<T, U, V> (this ISequence<T> seq,
			Func<T, ISequence<U>> project, Func<T, U, V> select)
		{
			return seq.Map (t => project (t).Map (u => select (t, u))).Collect(Fun.Identity);
		}

		public static ISequence<T> Where<T> (this ISequence<T> seq, Func<T, bool> predicate)
		{
			return seq.Filter (predicate);
		}
	}
}
