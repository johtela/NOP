namespace NOP.IO
{
	using System;
	using System.Collections.Generic;
	using Parsing;

	public static class ConvertLiteral
	{
		private static Dictionary<Type, Func<object, string>> _converters =
			new Dictionary<Type, Func<object, string>>
		{
			{ typeof (string), str => string.Format ("\"{0}\"", str) }
		};

		private static Dictionary<Type, Parser<object, char>> _parsers;

		static ConvertLiteral ()
		{
			var quote = StringParser.Char ('"');
			
			var stringParser = StringParser.NoneOf ('"').ManyChars ()
				.Bracket (quote, quote).Cast<string, object, char> ();

			var boolParser =
				(from val in StringParser.String ("True").Plus (StringParser.String ("False"))
				 select val == "True").Cast<bool, object, char> ();

			_parsers = new Dictionary<Type, Parser<object, char>>
			{
				{ typeof (bool), boolParser },
				{ typeof (string), stringParser },
				{ typeof (int), StringParser.Integer ().Cast<int, object, char> () }
			};
		}

		public static string ToString (object obj)
		{
			Func<object, string> converter;

			return _converters.TryGetValue (obj.GetType (), out converter) ?
				converter (obj) : obj.ToString ();
		}

		public static Parser<object, char> GetParser (Type type)
		{
			Parser<object, char> parser;

			if (!_parsers.TryGetValue (type, out parser))
				throw new NOPException ("Could not find parser for literal type " + type.FullName);
			return parser;
		}
	}
}
