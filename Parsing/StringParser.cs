namespace NOP.Parsing
{
	using System;
	using System.Text;
	using NOP.Collections;

	/// <summary>
	/// Parser combinator for characters and strings.
	/// </summary>
	public static class StringParser
	{
		/// <summary>
		/// Parse a given character.
		/// </summary>
		public static Parser<char, char, P> Char<P> (char x)
		{
			return Parser.Satisfy<char, P> (y => x == y);
		}

		/// <summary>
		/// Parse a number [0-9]
		/// </summary>
		public static Parser<char, char, P> Number<P> ()
		{
			return Parser.Satisfy<char, P> (char.IsNumber);
		}

		/// <summary>
		/// Parse a lower case character [a-z]
		/// </summary>
		public static Parser<char, char, P> Lower<P> ()
		{
			return Parser.Satisfy<char, P> (char.IsLower);
		}

		/// <summary>
		/// Parse an upper case character [A-Z]
		/// </summary>
		public static Parser<char, char, P> Upper<P> ()
		{
			return Parser.Satisfy<char, P> (char.IsUpper);
		}

		/// <summary>
		/// Parse any letter.
		/// </summary>
		public static Parser<char, char, P> Letter<P> ()
		{
			return Parser.Satisfy<char, P> (char.IsLetter);
		}

		/// <summary>
		/// Parse on alphanumeric character.
		/// </summary>
		public static Parser<char, char, P> AlphaNumeric<P> ()
		{
			return Parser.Satisfy<char, P> (char.IsLetterOrDigit);
		}

		/// <summary>
		/// Parse a word (sequence of consequtive letters)
		/// </summary>
		/// <returns></returns>
		public static Parser<string, char, P> Word<P> ()
		{
			return from xs in Letter<P> ().Many ()
				   select xs.ToString ("", "", "");
		}

		/// <summary>
		/// Parse a given sequence of characters.
		/// </summary>
		public static Parser<IStream<char>, char, P> CharStream<P> (IStream<char> str)
		{
			return str.IsEmpty ? str.ToParser<IStream<char>, char, P> () :
				Char<P> (str.First).Seq (
				CharStream<P> (str.Rest).Seq (
				str.ToParser<IStream<char>, char, P> ()));
		}

		/// <summary>
		/// Parse a given string.
		/// </summary>
		public static Parser<string, char, P> String<P> (string str)
		{
			return from seq in CharStream<P> (LazyList.FromEnumerable (str))
				   select str;
		}

		/// <summary>
		/// Parse a positive integer without a leading '+' character.
		/// </summary>
		public static Parser<int, char, P> PositiveInteger<P> ()
		{
			return (from x in Number<P> ()
					select x - '0').ChainLeft1 (
					Parser.ToParser<Func<int, int, int>, char, P> (
						(m, n) => 10 * m + n));
		}

		/// <summary>
		/// Creates a parser that skips whitespace, i.e. just consumes white space 
		/// from the sequence but does not return anything.
		/// </summary>
		public static Parser<Unit, char, P> WhiteSpace<P> ()
		{
			return from _ in Parser.Satisfy<char, P> (char.IsWhiteSpace).Many1 ()
				   select Unit.Void;
		}

		public static Parser<Unit, char, P> Comment<P> ()
		{
			var nl = Environment.NewLine;
			var eol = nl[nl.Length - 1];

			return from x in String<P> ("//")
				   from y in Parser.Satisfy<char, P> (c => c != eol).Many ()
				   select Unit.Void;
		}

		public static Parser<Unit, char, P> Junk<P> ()
		{
			return from _ in WhiteSpace<P> ().Plus (Comment<P> ()).Many ()
				   select Unit.Void;
		}

		public static Parser<T, char, P> SkipJunk<T, P> (this Parser<T, char, P> parser)
		{
			return from _ in Junk<P> ()
				   from v in parser
				   select v;
		}

		public static Parser<T, char, P> Token<T, P> (this Parser<T, char, P> parser)
		{
			return from v in parser
				   from _ in Junk<P> ()
				   select v;
		}

		public static Parser<string, char, P> Identifier<P> ()
		{
			return from x in Letter<P> ()
				   from xs in AlphaNumeric<P> ().Many ()
				   select (x | xs).ToString ("", "", "");
		}

		public static Parser<string, char, P> Identifier<P> (Set<string> identifiers)
		{
			return (from x in Identifier<P> ()
					where identifiers.Contains (x)
					select x).Token ();
		}
	}
}
