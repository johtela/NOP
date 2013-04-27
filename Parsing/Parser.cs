namespace NOP.Parsing
{
	using System;
	using NOP.Collections;
using System.Text;

	public delegate Tuple<T, ISequence<S>> Parser<T, S> (ISequence<S> seq);

	public static class Parser
	{
		public static Parser<U, S> Bind<T, U, S> (this Parser<T, S> parser, Func<T, Parser<U, S>> func)
		{
			return seq =>
			{
				var res = parser (seq);
				return res == null ? null : func (res.Item1) (res.Item2);
			};
		}

		public static Parser<U, S> Seq<T, U, S> (this Parser<T, S> parser, Parser<U, S> other)
		{
			return seq =>
			{
				var res = parser (seq);
				return res == null ? null : other (res.Item2);
			};
		}

		public static Parser<T, S> ToParser<T, S> (this T value)
		{
			return seq => Tuple.Create (value, seq);
		}

		public static Parser<T, S> Fail<T, S> ()
		{
			return seq => null;
		}

		public static Parser<T, T> Item<T> ()
		{
			return seq => seq.IsEmpty ? null : Tuple.Create (seq.First, seq.Rest);
		}

		public static Parser<T, T> Sat<T> (Func<T, bool> predicate)
		{
			return Item<T> ().Bind (x => predicate (x) ? ToParser<T, T> (x) : Fail<T, T> ());
		}

		public static Parser<T, U> Plus<T, U> (this Parser<T, U> parser, Parser<T, U> other)
		{
			return seq => parser (seq) ?? other (seq);
		}

		public static Parser<StrictList<T>, T> Many<T> (this Parser<T, T> parser)
		{
			return parser.Bind (x =>
				Many (parser).Bind (xs =>
				ToParser<StrictList<T>, T> (x | xs))).Plus (
				ToParser<StrictList<T>, T> (StrictList<T>.Empty));
		}

		public static Parser<StrictList<T>, T> Many1<T> (this Parser<T, T> parser)
		{
			return parser.Bind (x =>
				Many (parser).Bind (xs =>
				ToParser<StrictList<T>, T> (x | xs)));
		}

		public static Parser<StrictList<T>, T> SeparatedBy1<T, U> (this Parser<T, T> parser, 
			Parser<U, T> separator)
		{
			return from x in parser
				   from xs in
					    (from y in separator.Seq (parser)
						 select y).Many ()
				   select x | xs;
		}

		public static Parser<U, S> Select<T, U, S> (this Parser<T, S> parser, Func<T, U> select)
		{
			return parser.Bind (x => select(x).ToParser<U, S> ());
		}

		public static Parser<V, S> SelectMany<T, U, V, S> (this Parser<T, S> parser,
			Func<T, Parser<U, S>> project, Func<T, U, V> select)
		{
			return parser.Bind (x => project (x).Bind (y => select (x, y).ToParser<V, S> ())); 
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

		public static Parser<char, char> Letter ()
		{
			return Sat<char> (char.IsLetter);
		}

		public static Parser<char, char> AlphaNumeric ()
		{
			return Sat<char> (char.IsLetterOrDigit);
		}

		public static Parser<string, char> Word ()
		{
			return Letter ().Many ().Bind (xs => ToParser<string, char> (xs.ToString ("", "", "")));
		}

		public static Parser<ISequence<char>, char> String (ISequence<char> str)
		{
			return str.IsEmpty ? ToParser<ISequence<char>, char> (str) :
				Char (str.First).Seq (
				String (str.Rest).Seq (
				ToParser<ISequence<char>, char> (str)));
		}
	}
}
