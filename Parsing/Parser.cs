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
	public delegate Consumed<T, S, P> Parser<T, S, P> (Input<S, P> input);

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
		public static Parser<U, S, P> Bind<T, U, S, P> (this Parser<T, S, P> parser, Func<T, Parser<U, S, P>> func)
		{
			return input =>
			{
				var res1 = parser (input);
				if (res1.IsEmpty)
				{
					if (res1.Reply)
					{
						var res2 = func (res1.Reply.Result) (res1.Reply.Input);
						return res2.IsEmpty ?
							new Empty<U, S, P> (Lazy.Create (res2.Reply.MergeExpected (res1.Reply))) :
							res2;
					}
					return new Empty<U, S, P> (
						Lazy.Create (Reply<U, S, P>.Fail (res1.Reply.Input, res1.Reply.Found, res1.Reply.Expected)));
				} 
				else
					return new Consumed<U, S, P> (Lazy.Create (() =>
					{
						if (res1.Reply)
						{
							var res2 = func (res1.Reply.Result) (res1.Reply.Input);
							return res2.IsEmpty ?
							 		res2.Reply.MergeExpected (res1.Reply) :
									res2.Reply;
						}
						return Reply<U, S, P>.Fail (res1.Reply.Input, res1.Reply.Found, res1.Reply.Expected);
					}));
			};
		}

		/// <summary>
		/// The monadic sequencing. Runs the first parser, and if it succeeds, runs the second
		/// parser ignoring the result of the first one. Corresponds to Haskell's >> operator.
		/// </summary>
		public static Parser<U, S, P> Seq<T, U, S, P> (this Parser<T, S, P> parser, Parser<U, S, P> other)
		{
			return parser.Bind (_ => other);
		}

		/// <summary>
		/// The monadic return. Lifts a value to the parser monad, i.e. creates
		/// a parser that just returns a value without consuming any input.
		/// </summary>
		public static Parser<T, S, P> ToParser<T, S, P> (this T value)
		{
			return input => new Empty<T, S, P> (Lazy.Create (Reply<T, S, P>.Ok (value, input)));
		}

		/// <summary>
		/// Creates a parser that reads one item from input and returns it, if
		/// it satisfies a given predicate; otherwise the parser will fail.
		/// </summary>
		public static Parser<T, T, P> Satisfy<T, P> (Func<T, bool> predicate)
		{
			return input =>
			{
				if (input.IsEmpty)
					return new Empty<T, T, P> (Lazy.Create (Reply<T, T, P>.Fail (input, "end of input")));
				var item = input.First;
				return predicate (item) ?
					new Consumed<T, T, P> (Lazy.Create (Reply<T, T, P>.Ok (item, input.Rest))) :
					new Empty<T, T, P> (Lazy.Create (Reply<T, T, P>.Fail (input, item.ToString ())));
			};
		}

		/// <summary>
		/// The monadic plus operation. Creates a parser that runs the first parser, and if
		/// that fails, runs the second one. Corresponds to the | operation in BNF grammars.
		/// </summary>
		public static Parser<T, S, P> Plus<T, S, P> (this Parser<T, S, P> parser, Parser<T, S, P> other)
		{
			return input =>
			{
				var res1 = parser (input);
				if (res1.IsEmpty && !res1.Reply)
				{
					var res2 = other (input);
					return res2.IsEmpty ?
						new Empty<T, S, P> (Lazy.Create (res2.Reply.MergeExpected (res1.Reply))) :
						res2;
				}
				return res1;
			};
		}

		/// <summary>
		/// Select extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static Parser<U, S, P> Select<T, U, S, P> (this Parser<T, S, P> parser, Func<T, U> select)
		{
			return parser.Bind (x => select (x).ToParser<U, S, P> ());
		}

		/// <summary>
		/// SelectMany extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static Parser<V, S, P> SelectMany<T, U, V, S, P> (this Parser<T, S, P> parser,
			Func<T, Parser<U, S, P>> project, Func<T, U, V> select)
		{
			return parser.Bind (x => project (x).Bind (y => select (x, y).ToParser<V, S, P> ()));
		}

		/// <summary>
		/// Where extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static Parser<T, S, P> Where<T, S, P> (this Parser<T, S, P> parser, Func<T, bool> predicate)
		{
			return parser.Bind (x => predicate (x) ? x.ToParser<T, S, P> () : null);
		}

		/// <summary>
		/// Creates a parser that will run a given parser zero or more times. The results
		/// of the input parser are added to a list.
		/// </summary>
		public static Parser<StrictList<T>, S, P> Many<T, S, P> (this Parser<T, S, P> parser)
		{
			return (from x in parser
					from xs in parser.Many ()
					select x | xs)
					.Plus (StrictList<T>.Empty.ToParser<StrictList<T>, S, P> ());
		}

		/// <summary>
		/// Creates a parser that will run a given parser one or more times. The results
		/// of the input parser are added to a list.
		/// </summary>
		public static Parser<StrictList<T>, S, P> Many1<T, S, P> (this Parser<T, S, P> parser)
		{
			return from x in parser
				   from xs in parser.Many ()
				   select x | xs;
		}

		/// <summary>
		/// Creates a parser that will read a list of items separated by a separator.
		/// The list needs to have at least one item.
		/// </summary>
		public static Parser<StrictList<T>, S, P> SeparatedBy1<T, U, S, P> (this Parser<T, S, P> parser,
			Parser<U, S, P> separator)
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
		public static Parser<StrictList<T>, S, P> SeparatedBy<T, U, S, P> (this Parser<T, S, P> parser,
			Parser<U, S, P> separator)
		{
			return SeparatedBy1 (parser, separator).Plus (
				StrictList<T>.Empty.ToParser<StrictList<T>, S, P> ());
		}

		/// <summary>
		/// Creates a parser the reads a bracketed input.
		/// </summary>
		public static Parser<T, S, P> Bracket<T, U, V, S, P> (this Parser<T, S, P> parser,
			Parser<U, S, P> open, Parser<V, S, P> close)
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
		public static Parser<T, S, P> ChainLeft1<T, S, P> (this Parser<T, S, P> parser,
			Parser<Func<T, T, T>, S, P> operation)
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
		public static Parser<T, S, P> ChainRight1<T, S, P> (this Parser<T, S, P> parser,
			Parser<Func<T, T, T>, S, P> operation)
		{
			return parser.Bind (x =>
				   (from f in operation
					from y in ChainRight1 (parser, operation)
					select f (x, y))
					.Plus (x.ToParser<T, S, P> ())
			);
		}

		/// <summary>
		/// Creates a parser that reads an expression with multiple terms separated
		/// by an operator. The operator is returned as a function and the terms are
		/// evaluated left to right. If the parsing of the expression fails, the value
		/// given as an argument is returned as a parser.
		/// </summary>
		public static Parser<T, S, P> ChainLeft<T, S, P> (this Parser<T, S, P> parser,
			Parser<Func<T, T, T>, S, P> operation, T value)
		{
			return parser.ChainLeft1 (operation).Plus (value.ToParser<T, S, P> ());
		}

		/// <summary>
		/// Creates a parser that reads an expression with multiple terms separated
		/// by an operator. The operator is returned as a function and the terms are
		/// evaluated right to left. If the parsing of the expression fails, the value
		/// given as an argument is returned as a parser.
		/// </summary>
		public static Parser<T, S, P> ChainRight<T, S, P> (this Parser<T, S, P> parser,
			Parser<Func<T, T, T>, S, P> operation, T value)
		{
			return parser.ChainRight1 (operation).Plus (value.ToParser<T, S, P> ());
		}

		/// <summary>
		/// Create a combined parser that will parse any of the given operators. 
		/// The operators are specified in a seqeunce which contains (parser, result)
		/// pairs. If the parser succeeds the result is returned, otherwise the next 
		/// parser in the sequence is tried.
		/// </summary>
		public static Parser<U, S, P> Operators<T, U, S, P> (ISequence<Tuple<Parser<T, S, P>, U>> ops)
		{
			return ops.Map (op => from _ in op.Item1
								  select op.Item2).ReduceLeft1 (Plus);
		}
	}
}
