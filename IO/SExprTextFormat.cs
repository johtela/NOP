namespace NOP.IO
{
	using System;
	using Parsing;

	public class SExprTextFormat
	{
		private static Parser<SExpr, char> sexpr;

		static SExprTextFormat ()
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
						 select SExpr.Sym ((x | xs).ToString ());

			var literal = from ob in StringParser.Char ('{')
						  from t in type
						  from col in StringParser.Char (':')
						  from value in ConvertLiteral.GetParser (t)
						  from cb in StringParser.Char ('}')
						  select SExpr.Lit (value);

			sexpr = list.Plus (symbol).Plus (literal).SkipJunk ();
		}

		public static SExpr Parse (Input<char> input)
		{
			return sexpr.Parse (input);
		}
	}
}
