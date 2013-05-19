namespace NOP.Grammar
{
	using System;
	using System.Linq;
	using NOP.Parsing;
	using Seq = NOP.Collections.Sequence<SExpr>;

	public static class SExprParser
	{
		public static Parser<SExpr, Seq> SExpr ()
		{
			return Parser.Satisfy<Seq> (seq => !seq.IsEmpty).Bind (
				seq => seq.First.ToParser<SExpr, Seq> ());
		}

		public static Parser<Expression._Symbol, Seq> Symbol ()
		{
			return (from sexp in SExpr ()
					where sexp is SExpr.Symbol
					select new Expression._Symbol (sexp as SExpr.Symbol)).Label ("symbol");
		}

		public static Parser<Expression._Symbol, Seq> Symbol (string name)
		{
			return (from sym in Symbol ()
					where sym.Symbol.Name == name
					select sym).Label ("symbol: " + name);
		}

		public static Parser<Expression._Literal, Seq> Literal ()
		{
			return (from sexp in SExpr ()
					where sexp is SExpr.Literal
					select new Expression._Literal (sexp as SExpr.Literal)).Label ("literal");
		}

		public static Parser<SExpr.List, Seq> List ()
		{
			return (from sexp in SExpr ()
					where sexp is SExpr.List
					select sexp as SExpr.List).Label ("list");
		}

		public static Parser<Seq, Seq> Eol ()
		{
			return Parser.Satisfy<Seq> (seq => seq.IsEmpty).Label ("end of list");
		}

		public static Parser<Definition, Seq> Define ()
		{
			return Type ().Plus (Member ());
		}

		public static Parser<Definition, Seq> Type ()
		{
			return from lst in List ()
				   from sym in Symbol ("type").Seq (Symbol ())
				   from defs in Define ().Many1 ()
				   from eol in Eol ()
				   select Definition.Type (lst, sym, defs);
		}

		public static Parser<Definition, Seq> Member ()
		{
			return from lst in List ()
				   from sym in Symbol ("def").Seq (Symbol ())
				   from var in Variable ()
				   from eol in Eol ()
				   select Definition.Member (lst, var, null);
		}

		public static Parser<VariableDefinition, Seq> Variable ()
		{
			return (from sym in Symbol ()
					select new VariableDefinition (sym.SExp, sym, null)).Plus (
						from lst in List ()
						from sym in Symbol ()
						from tref in TypeRef ()
						from eol in Eol ()
						select new VariableDefinition (lst, sym, tref))
						.Label ("variable definition");
		}

		public static Parser<TypeReference, Seq> TypeRef ()
		{
			return SimpleType ().Plus (GenericType ()).Plus (FunctionType ());
		}

		public static Parser<TypeReference, Seq> SimpleType ()
		{
			return (from sym in Symbol ()
					select TypeReference.Simple (sym)).Label ("type reference");
		}

		public static Parser<TypeReference, Seq> GenericType ()
		{
			return (from lst in List ()
					from sym in Symbol ()
					from tref in TypeRef ().Many1 ()
					from eol in Eol ()
					select TypeReference.Generic (lst, sym, tref)).Label ("generic type reference");
		}

		public static Parser<TypeReference, Seq> FunctionType ()
		{
			return (from lst in List ()
					from arr in Symbol ("->")
					from arg in TypeRef ()
					from res in TypeRef ()
					select TypeReference.Function (lst, arg, res)).Label ("function type reference");
		}
	}
}
