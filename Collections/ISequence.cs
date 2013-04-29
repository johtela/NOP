namespace NOP.Collections
{
	using System;

	/// <summary>
	/// Immutable sequence that can be strict or lazy. A sequence
	/// has to implement also IFunctor to provide mapping support
	/// plus IReducible to provide folding functions.
	/// </summary>
	public interface ISequence<T> : IStream<T>, IReducible<T>
	{
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
	/// Linq extension methods for sequences.
	/// </summary>
	public static class SequenceExtensions
	{
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
