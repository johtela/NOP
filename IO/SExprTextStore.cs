namespace NOP.IO
{
	using System;
	using System.IO;
	using Parsing;

	public class SExprTextStore : SExprStore<TextReader, TextWriter>
	{
		private static Parser<SExpr, char> sexpr;

		static SExprTextStore ()
		{
			var type = StringParser.NoneOf (':').ManyChars ().Bind (typename =>
						{
							var t = Type.GetType (typename);
							return t != null ?
								t.ToParser<Type, char> () :
								Parser.Fail<Type, char> (typename, "valid type name");
						});

			var list = from op in StringParser.Char ('(')
					   from sexps in sexpr.Many ()
					   from cp in StringParser.Char (')').SkipJunk ()
					   select SExpr.Lst (sexps);

			var symbol = from x in StringParser.Letter ()
						 from xs in StringParser.AlphaNumeric ().Plus (StringParser.Char ('?')).Many ()
						 select SExpr.Sym (new Name((x | xs).ToString ()));

			var literal = from ob in StringParser.Char ('{')
						  from t in type
						  from col in StringParser.Char (':')
						  from value in ConvertLiteral.GetParser (t)
						  from cb in StringParser.Char ('}')
						  select SExpr.Lit (value);

			sexpr = list.Plus (symbol).Plus (literal).SkipJunk ();
		}

		public override void Write (SExpr sexp, TextWriter writer)
		{
			writer.Write (sexp.ToString ());
		}

		public override SExpr Read (TextReader reader)
		{
			var input = Input.FromString (reader.ReadToEnd ());
			return sexpr.Parse (input);
		}
	}
}
