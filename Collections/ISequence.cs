﻿namespace NOP.Collections
{
	using System;

	/// <summary>
	/// Immutable sequence that can be strict or lazy. A sequence
	/// has to implement IReducible to provide folding functions.
	/// </summary>
	public interface ISequence<T> : IStream<T>, IReducible<T>
	{
		/// <summary>
		/// Concatenate two sequences.
		/// </summary>
		ISequence<T> Concat (ISequence<T> other);

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

		/// <summary>
		/// Zip two lists to a list of tuples.
		/// </summary>
		ISequence<Tuple<T, U>> ZipWith<U> (ISequence<U> other);
	}

	/// <summary>
	/// Linq extension methods for sequences.
	/// </summary>
	public static class Seq
	{
		public static S Reverse<S, T> (this S seq) where S : ISequence<T>
		{
			var b = Strm.Builder<S, T> ();
			return seq.ReduceLeft (b.Empty, (s, i) => b.Cons (i, s));
		}

		public static S AddToFront<S, T> (this S seq, T item) where S : ISequence<T>
		{
			return Strm.Cons<S, T> (item, seq);
		}

		public static S AddToBack<S, T> (this S seq, T item) where S : ISequence<T>
		{
			return (S)seq.Concat (Strm.Cons<S, T> (item));
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

		public static T ReduceLeft1<T> (this ISequence<T> seq, Func<T, T, T> func)
		{
			return ((ISequence<T>)seq.Rest).ReduceLeft (seq.First, func);
		}
	}
}
