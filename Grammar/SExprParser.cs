﻿namespace NOP.Grammar
{
	using System;
	using System.Linq;
	using NOP.Parsing;
	using Seq = NOP.Collections.Sequence<SExpr>;

	public static class SExprParser
	{
		public static Parser<T, Seq> SExpr<T> () where T : SExpr
		{
			return Parser.Satisfy<Seq> (seq => !seq.IsEmpty && seq.First is T).Bind (seq => 
				(seq.First as T).ToParser<T, Seq> ());
		}

		public static Parser<T, Seq> SExpr<T> (Func<T, bool> predicate) where T : SExpr
		{
			return Parser.Satisfy<Seq> (seq => !seq.IsEmpty && seq.First is T && predicate (seq.First as T))
				.Bind (seq => (seq.First as T).ToParser<T, Seq> ());
		}

		public static Parser<Expression._Symbol, Seq> Symbol ()
		{
			return SExpr<SExpr.Symbol> ().Bind (sexp => 
				new Expression._Symbol (sexp).ToParser<Expression._Symbol, Seq> ())
				.Label ("symbol");
		}

		public static Parser<Expression._Symbol, Seq> Symbol (string name)
		{
			return SExpr<SExpr.Symbol> (sym => sym.Name == name).Bind (sexp =>
				new Expression._Symbol (sexp).ToParser<Expression._Symbol, Seq> ())
				.Label ("symbol: " + name);
		}

		public static Parser<Expression._Literal, Seq> Literal ()
		{
			return SExpr<SExpr.Literal> ().Bind (sexp =>
				new Expression._Literal (sexp).ToParser<Expression._Literal, Seq> ())
				.Label ("literal");
		}

		public static Parser<SExpr.List, Seq> List ()
		{
			return SExpr<SExpr.List> ().Label ("list");
		}

		public static Parser<Seq, Seq> Eol ()
		{
			return Parser.Satisfy<Seq> (seq => seq.IsEmpty).Label ("end of list");
		}

		public static Parser<T, Seq> List<T> (Parser<T, Seq> parser)
		{
			return parser.Bracket (List (), Eol ());
		}

		public static Parser<Definition, Seq> Define ()
		{
			return from lst in List ()
				   from def in Type (lst)
						.Plus (Member (lst))
				   from eol in Eol ()
				   select def;
		}

		public static Parser<Definition, Seq> Type (SExpr lst)
		{
			return from sym in Symbol ("type").Seq (Symbol ())
				   from defs in Define ().Many1 ()
				   select Definition.Type (lst, sym, defs);
		}

		public static Parser<Definition, Seq> Member (SExpr lst)
		{
			return from sym in Symbol ("def").Seq (Symbol ())
				   from var in Variable ()
				   from expr in Expr ()
				   select Definition.Member (lst, var, expr);
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
			return (from lst in List ()
					from typ in FunctionType (lst)
						.Plus (GenericType (lst))
					from eol in Eol ()
					select typ)
				.Plus (SimpleType ());
		}

		public static Parser<TypeReference, Seq> SimpleType ()
		{
			return (from sym in Symbol ()
					select TypeReference.Simple (sym)).Label ("type reference");
		}

		public static Parser<TypeReference, Seq> GenericType (SExpr lst)
		{
			return (from sym in Symbol ()
					from tref in TypeRef ().Many1 ()
					select TypeReference.Generic (lst, sym, tref)).Label ("generic type reference");
		}

		public static Parser<TypeReference, Seq> FunctionType (SExpr lst)
		{
			return (from arr in Symbol ("->")
					from arg in TypeRef ()
					from res in TypeRef ()
					select TypeReference.Function (lst, arg, res)).Label ("function type reference");
		}

		public static Parser<Expression, Seq> Expr ()
		{
			return (from lst in List ()
					from expr in If (lst)
						.Plus (Lambda (lst))
						.Plus (Let (lst))
						.Plus (Quoted (lst))
						.Plus (Set (lst))
						.Plus (Application (lst))
					from eol in Eol ()
					select expr)
				.Plus (Literal ().Cast<Expression._Literal, Expression, Seq> ())
				.Plus (Symbol ().Cast<Expression._Symbol, Expression, Seq> ());
		}

		public static Parser<Expression, Seq> Application (SExpr lst)
		{
			return from fun in Expr ()
				   from pars in Expr ().Many ()
				   select Expression.Application (lst, fun, pars);
		}

		public static Parser<Expression, Seq> If (SExpr lst)
		{
			return from cnd in Symbol ("if").Seq (Expr ())
				   from thn in Expr ()
				   from els in Expr ()
				   select Expression.If (lst, cnd, thn, els);
		}

		public static Parser<Expression, Seq> Lambda (SExpr lst)
		{
			return from pars in Symbol ("lambda").Seq (List (Variable ().Many ()))
				   from body in Expr ()
				   select Expression.Lambda (lst, pars, body);
		}

		public static Parser<Expression, Seq> Let (SExpr lst)
		{
			return from var in Symbol ("let").Seq (Variable ())
				   from val in Expr ()
				   from body in Expr ()
				   select Expression.Let (lst, var, val, body);
		}

		public static Parser<Expression, Seq> Quoted (SExpr lst)
		{
			return from expr in Symbol ("quote").Seq (Expr ())
				   select Expression.Quoted (lst, expr);
		}

		public static Parser<Expression, Seq> Set (SExpr lst)
		{
			return from sym in Symbol ("set!").Seq (Symbol ())
				   from val in Expr ()
				   select Expression.Set (lst, sym, val);
		}
	}
}
