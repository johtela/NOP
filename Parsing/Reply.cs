namespace NOP.Parsing
{
	using System;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	public abstract class Reply<T, S, P>
	{
		public readonly Input<S, P> Input;
		public readonly string Found;
		public readonly LazyList<string> Expected;
		public abstract T Result { get; }

		private Reply (Input<S, P> input, string found, LazyList<string> expected)
		{
			Input = input;
			Found = found;
			Expected = expected;							
		}

		public abstract Reply<T, S, P> MergeExpected<U> (Reply<U, S, P> other);

		private class Success : Reply<T, S, P>
		{
			private readonly T _result;

			public Success (T result, Input<S, P> input, string found, LazyList<string> expected) :
				base (input, found, expected)
			{
				_result = result;
			}

			public override T Result
			{
				get { return _result; }
			}

			public override Reply<T, S, P> MergeExpected<U> (Reply<U, S, P> other)
			{
				return new Success (Result, Input, Found, other.Expected + Expected);
			}
		}

		private class Failure : Reply<T, S, P>
		{
			public Failure (Input<S, P> input, string found, LazyList<string> expected) :
				base (input, found, expected) { }

			public override T Result
			{
				get { throw new ParseError ("Parse failed. No result available."); }
			}

			public override Reply<T, S, P> MergeExpected<U> (Reply<U, S, P> other)
			{
				return new Failure (Input, Found, other.Expected + Expected);
			}
		}

		public static Reply<T, S, P> Ok (T result, Input<S, P> input)
		{
			return new Success (result, input, string.Empty, LazyList<string>.Empty);
		}

		public static Reply<T, S, P> Ok (T result, Input<S, P> input, string found, 
			LazyList<string> expected)
		{
			return new Success (result, input, found, expected);
		}

		public static Reply<T, S, P> Fail (Input<S, P> input, string found) 
		{
			return new Failure (input, found, LazyList<string>.Empty);
		}

		public static Reply<T, S, P> Fail (Input<S, P> input, string found, LazyList<string> expected) 
		{
			return new Failure (input, found, expected);
		}

		public static implicit operator bool (Reply<T, S, P> reply)
		{
			return reply is Success;
		}
	}
}