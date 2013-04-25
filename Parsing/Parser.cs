namespace NOP.Parsing
{
	using System;
	using NOP.Collections;

	public delegate Tuple<T, ISequence<U>> Parser<T, U> (ISequence<U> seq);

	public static class Parser
	{
		public static Parser<V, U> Bind<T, U, V> (this Parser<T, U> parser, Func<T, Parser<V, U>> func)
		{
			return seq =>
			{
				var res = parser (seq);
				return res == null ? null : func (res.Item1) (res.Item2);
			};
		}

		public static Parser<T, U> Return<T, U> (T value)
		{
			return seq => Tuple.Create (value, seq);
		}

		public static Parser<T, U> Fail<T, U> ()
		{
			return seq => null;
		}

		public static Parser<T, T> Item<T> ()
		{
			return seq => seq.IsEmpty ? null : Tuple.Create (seq.First, seq.Rest);
		}

		public static Parser<T, T> Sat<T> (Func<T, bool> predicate)
		{
			return Item<T> ().Bind (x => predicate (x) ? Return<T, T> (x) : Fail<T, T> ());
		}

		public static Parser<char, char> Char (char x)
		{
			return Sat<char> (y => x == y);
		}

		public static Parser<char, char> Number ()
		{
			return Sat<char> (char.IsNumber);
		}

		public static Parser<char, char> Lower ()
		{
			return Sat<char> (char.IsLower);
		}

		public static Parser<char, char> Upper ()
		{
			return Sat<char> (char.IsUpper);
		}

		public static Parser<T, U> Plus<T, U> (this Parser<T, U> parser, Parser<T, U> other)
		{
			return seq => parser (seq) ?? other (seq);
		}
	}
}
