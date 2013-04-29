namespace NOP.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public interface IStream<T>
	{
		/// <summary>
		/// The first element of the sequence.
		/// </summary>
		T First { get; }

		/// <summary>
		/// The rest of the sequence.
		/// </summary>
		IStream<T> Rest { get; }

		/// <summary>
		/// Is the sequence empty.
		/// </summary>
		bool IsEmpty { get; }
	}

	public static class StreamExtensions
	{
		/// <summary>
		/// Count the length, i.e. the number of items, in the sequence.
		/// </summary>
		public static int Length<T> (this IStream<T> seq)
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
		public static IStream<T> FindNext<T> (this IStream<T> seq, T item)
		{
			while (!seq.IsEmpty && !seq.First.Equals (item))
				seq = seq.Rest;
			return seq;
		}

		/// <summary>
		/// Find an item in the list that matches a predicate.
		/// </summary>
		public static IStream<T> FindNext<T> (this IStream<T> seq, Predicate<T> predicate)
		{
			while (!seq.IsEmpty && !predicate (seq.First))
				seq = seq.Rest;
			return seq;
		}

		/// <summary>
		/// Drop n items from the sequence.
		/// </summary>
		public static IStream<T> Drop<T> (this IStream<T> seq, int n)
		{
			while (n-- > 0)
				seq = seq.Rest;
			return seq;
		}

		/// <summary>
		/// Return the position of the specified item.
		/// </summary>
		public static int IndexOf<T> (this IStream<T> seq, T item)
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
		public static bool EqualTo<T> (this IStream<T> seq, IStream<T> other, Func<T, T, bool> equals)
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
		public static bool EqualTo<T> (this IStream<T> seq, IStream<T> other)
		{
			return seq.EqualTo (other, (i1, i2) => i1.Equals (i2));
		}

		/// <summary>
		/// Returns a string representing the list. Gets open, close bracket, and separtor as
		/// an argument.
		/// </summary>
		public static string ToString<T> (this IStream<T> seq, string openBracket, string closeBracket, string separator)
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
		public static IEnumerable<T> ToEnumerable<T> (this IStream<T> seq)
		{
			while (!seq.IsEmpty)
			{
				yield return seq.First;
				seq = seq.Rest;
			}
		}
	}
}
