namespace NOP.Parsing
{
	using System;
	using System.Text;
	using NOP.Collections;

	/// <summary>
	/// A parser is a function that reads input from a sequence and returns a 
	/// parsed value. The types of the input stream and the parsed value are 
	/// generic, so effectively parser can read any stream and return any value.
	/// </summary>
	public delegate Reply<S> Parser<T, S> (Input<S> seq);

	/// <summary>
	/// Monadic parsing operations implemented as extensions methods for the
	/// Parser[T, S] delegate.
	/// </summary>
	public static class Parser
	{
		/// <summary>
		/// The monadic bind. Runs the first parser, and if it succeeds, feeds the
		/// result to the second parser. Corresponds to Haskell's >>= operator.
		/// </summary>
		public static Parser<U, S> Bind<T, U, S> (this Parser<T, S> parser, Func<T, Parser<U, S>> func)
		{
			return seq =>
			{
				var res = parser (seq);
				if (res is Reply<S>.Success<T>)
				{
					var success = res as Reply<S>.Success<T>;
					return func (success.Result) (success.Input);
				} 
				return res;
//				return res == null ? null : func (res.Item1) (res.Item2);
			};
		}

		/// <summary>
		/// The monadic sequencing. Runs the first parser, and if it succeeds, runs the second
		/// parser ignoring the result of the first one. Corresponds to Haskell's >> operator.
		/// </summary>
		public static Parser<U, S> Seq<T, U, S> (this Parser<T, S> parser, Parser<U, S> other)
		{
			return seq =>
			{
				var res = parser (seq);
				return res == null ? null : other (res.Item2);
			};
		}

		/// <summary>
		/// The monadic return. Lifts a value to the parser monad, i.e. creates
		/// a parser that just returns a value without consuming any input.
		/// </summary>
		public static Parser<T, S> ToParser<T, S> (this T value)
		{
			return seq => Tuple.Create (value, seq);
		}

		/// <summary>
		/// Creates a parser that will always fail.
		/// </summary>
		public static Parser<T, S> Fail<T, S> ()
		{
			return seq => null;
		}

		/// <summary>
		/// Creates a parser that reads one item from the input and returns it.
		/// </summary>
		public static Parser<T, T> Item<T> ()
		{
			return seq => seq.IsEmpty ? null : Tuple.Create (seq.First, seq.Rest);
		}

		/// <summary>
		/// Creates a parser that reads one item from input and returns it, if
		/// it satisfies a given predicate; otherwise the parser will fail.
		/// </summary>
		public static Parser<T, T> Sat<T> (Func<T, bool> predicate)
		{
			return Item<T> ().Bind (x => predicate (x) ? ToParser<T, T> (x) : Fail<T, T> ());
		}

		/// <summary>
		/// The monadic plus operation. Creates a parser that runs the first parser, and if
		/// that fails, runs the second one. Corresponds to the | operation in BNF grammars.
		/// </summary>
		public static Parser<T, U> Plus<T, U> (this Parser<T, U> parser, Parser<T, U> other)
		{
			return seq => parser (seq) ?? other (seq);
		}

		/// <summary>
		/// Select extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static Parser<U, S> Select<T, U, S> (this Parser<T, S> parser, Func<T, U> select)
		{
			return parser.Bind (x => select (x).ToParser<U, S> ());
		}

		/// <summary>
		/// SelectMany extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static Parser<V, S> SelectMany<T, U, V, S> (this Parser<T, S> parser,
			Func<T, Parser<U, S>> project, Func<T, U, V> select)
		{
			return parser.Bind (x => project (x).Bind (y => select (x, y).ToParser<V, S> ()));
		}

		/// <summary>
		/// Where extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static Parser<T, S> Where<T, S> (this Parser<T, S> parser, Func<T, bool> predicate)
		{
			return parser.Bind (x => predicate (x) ? x.ToParser<T, S> () : null);
		}

		/// <summary>
		/// Creates a parser that will run a given parser zero or more times. The results
		/// of the input parser are added to a list.
		/// </summary>
		public static Parser<StrictList<T>, S> Many<T, S> (this Parser<T, S> parser)
		{
			return (from x in parser
					from xs in parser.Many ()
					select x | xs)
					.Plus (StrictList<T>.Empty.ToParser<StrictList<T>, S> ());
		}

		/// <summary>
		/// Creates a parser that will run a given parser one or more times. The results
		/// of the input parser are added to a list.
		/// </summary>
		public static Parser<StrictList<T>, S> Many1<T, S> (this Parser<T, S> parser)
		{
			return from x in parser
				   from xs in parser.Many ()
				   select x | xs;
		}

		/// <summary>
		/// Creates a parser that will read a list of items separated by a separator.
		/// The list needs to have at least one item.
		/// </summary>
		public static Parser<StrictList<T>, S> SeparatedBy1<T, U, S> (this Parser<T, S> parser, 
			Parser<U, S> separator)
		{
			return from x in parser
				   from xs in
					   (from y in separator.Seq (parser)
						select y).Many ()
				   select x | xs;
		}

		/// <summary>
		/// Creates a parser that will read a list of items separated by a separator.
		/// The list can also be empty.
		/// </summary>
		public static Parser<StrictList<T>, S> SeparatedBy<T, U, S> (this Parser<T, S> parser, 
			Parser<U, S> separator)
		{
			return SeparatedBy1 (parser, separator).Plus (
				StrictList<T>.Empty.ToParser<StrictList<T>, S> ());
		}

		/// <summary>
		/// Creates a parser the reads a bracketed input.
		/// </summary>
		public static Parser<T, S> Bracket<T, U, V, S> (this Parser<T, S> parser, 
			Parser<U, S> open, Parser<V, S> close)
		{
			return from o in open
				   from x in parser
				   from c in close
				   select x;
		}

		/// <summary>
		/// Creates a parser that reads an expression with multiple terms separated
		/// by an operator. The operator is returned as a function and the terms are
		/// evaluated left to right.
		/// </summary>
		public static Parser<T, S> ChainLeft1<T, S> (this Parser<T, S> parser, 
			Parser<Func<T, T, T>, S> operation)
		{
			return from x in parser
				   from fys in
					   (from f in operation
						from y in parser
						select new { f, y }).Many ()
				   select fys.ReduceLeft (x, (z, fy) => fy.f (z, fy.y));
		}

		/// <summary>
		/// Creates a parser that reads an expression with multiple terms separated
		/// by an operator. The operator is returned as a function and the terms are
		/// evaluated right to left.
		/// </summary>
		public static Parser<T, S> ChainRight1<T, S> (this Parser<T, S> parser,
			Parser<Func<T, T, T>, S> operation)
		{
			return parser.Bind (x =>
				   (from f in operation
					from y in ChainRight1 (parser, operation)
					select f (x, y))
					.Plus (x.ToParser<T, S> ())
			);
		}

		/// <summary>
		/// Creates a parser that reads an expression with multiple terms separated
		/// by an operator. The operator is returned as a function and the terms are
		/// evaluated left to right. If the parsing of the expression fails, the value
		/// given as an argument is returned as a parser.
		/// </summary>
		public static Parser<T, S> ChainLeft<T, S> (this Parser<T, S> parser, 
			Parser<Func<T, T, T>, S> operation, T value)
		{
			return parser.ChainLeft1 (operation).Plus (value.ToParser<T, S> ());
		}

		/// <summary>
		/// Creates a parser that reads an expression with multiple terms separated
		/// by an operator. The operator is returned as a function and the terms are
		/// evaluated right to left. If the parsing of the expression fails, the value
		/// given as an argument is returned as a parser.
		/// </summary>
		public static Parser<T, S> ChainRight<T, S> (this Parser<T, S> parser,
			Parser<Func<T, T, T>, S> operation, T value)
		{
			return parser.ChainRight1 (operation).Plus (value.ToParser<T, S> ());
		}

		/// <summary>
		/// Create a combined parser that will parse any of the given operators. 
		/// The operators are specified in a seqeunce which contains (parser, result)
		/// pairs. If the parser succeeds the result is returned, otherwise the next 
		/// parser in the sequence is tried.
		/// </summary>
		public static Parser<U, S> Operators<T, U, S> (ISequence<Tuple<Parser<T, S>, U>> ops)
		{
			return ops.Map (op => from _ in op.Item1
								  select op.Item2
			).ReduceLeft1 (Plus);
		}
	}
}
