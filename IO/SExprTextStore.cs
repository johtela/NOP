namespace NOP.IO
{
	using System.IO;
	using Parsing;

	public class SExprTextStore : SExprStore<TextReader, TextWriter>
	{
		public override void Write (SExpr sexp, TextWriter writer)
		{
			writer.Write (sexp.ToString ());
		}

		public override SExpr Read (TextReader reader)
		{
			var input = Input.FromString (reader.ReadToEnd ());
			return SExprTextFormat.Parse (input);
		}
	}
}
