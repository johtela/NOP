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
		public static Parser<char, char> Char (char x)
		{
			return Parser.Sat<char> (y => x == y);
		}

		/// <summary>
		/// Parse a number [0-9]
		/// </summary>
		public static Parser<char, char> Number ()
		{
			return Parser.Sat<char> (char.IsNumber);
		}

		/// <summary>
		/// Parse a lower case character [a-z]
		/// </summary>
		public static Parser<char, char> Lower ()
		{
			return Parser.Sat<char> (char.IsLower);
		}

		/// <summary>
		/// Parse an upper case character [A-Z]
		/// </summary>
		public static Parser<char, char> Upper ()
		{
			return Parser.Sat<char> (char.IsUpper);
		}

		/// <summary>
		/// Parse any letter.
		/// </summary>
		public static Parser<char, char> Letter ()
		{
			return Parser.Sat<char> (char.IsLetter);
		}

		/// <summary>
		/// Parse on alphanumeric character.
		/// </summary>
		public static Parser<char, char> AlphaNumeric ()
		{
			return Parser.Sat<char> (char.IsLetterOrDigit);
		}

		/// <summary>
		/// Parse a word (sequence of consequtive letters)
		/// </summary>
		/// <returns></returns>
		public static Parser<string, char> Word ()
		{
			return from xs in Letter ().Many ()
				   select xs.ToString ("", "", "");
		}

		/// <summary>
		/// Parse a given sequence of characters.
		/// </summary>
		public static Parser<ISequence<char>, char> CharSeq (ISequence<char> str)
		{
			return str.IsEmpty ? str.ToParser<ISequence<char>, char> () :
				Char (str.First).Seq (
				CharSeq (str.Rest).Seq (
				str.ToParser<ISequence<char>, char> ()));
		}

		/// <summary>
		/// Parse a given string.
		/// </summary>
		public static Parser<string, char> String (string str)
		{
			return from seq in CharSeq (LazyList.FromEnumerable (str))
				   select str;
		}

		/// <summary>
		/// Parse a positive integer without a leading '+' character.
		/// </summary>
		public static Parser<int, char> PositiveInteger ()
		{
			return (from x in Number ()
					select x - '0').ChainLeft1 (
					Parser.ToParser<Func<int, int, int>, char> (
						(m, n) => 10 * m + n));
		}

		/// <summary>
		/// Creates a parser that skips whitespace, i.e. just consumes white space 
		/// from the sequence but does not return anything.
		/// </summary>
		public static Parser<Unit, char> WhiteSpace ()
		{
			return from _ in Parser.Sat<char> (char.IsWhiteSpace).Many1 ()
				   select Unit.Void;
		}

		public static Parser<Unit, char> Comment ()
		{
			var nl = Environment.NewLine;
			var eol = nl[nl.Length - 1];

			return from x in String ("//")
				   from y in Parser.Sat<char> (c => c != eol).Many ()
				   select Unit.Void;
		}

		public static Parser<Unit, char> Junk ()
		{
			return from _ in WhiteSpace ().Plus (Comment ()).Many ()
				   select Unit.Void;
		}

		public static Parser<T, char> SkipJunk<T> (this Parser<T, char> parser)
		{
			return from _ in Junk ()
				   from v in parser
				   select v;
		}

		public static Parser<T, char> Token<T> (this Parser<T, char> parser)
		{
			return from v in parser
				   from _ in Junk ()
				   select v;
		}

		public static Parser<string, char> Identifier ()
		{
			return from x in Letter ()
				   from xs in AlphaNumeric ().Many ()
				   select (x | xs).ToString ("", "", "");
		}

		public static Parser<string, char> Identifier (Set<string> identifiers)
		{
			return (from x in Identifier ()
					where identifiers.Contains (x)
					select x).Token ();
		}
	}
}
