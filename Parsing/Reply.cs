namespace NOP.Parsing
{
	using System;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	public abstract class Reply<S>
	{
		public readonly Input<S> Input;
		public readonly string Found;
		public readonly IStream<string> Expected;

		private Reply (Input<S> input, string found, IStream<string> expected)
		{
			Input = input;
			Found = found;
			Expected = expected;							
		}

		public class Success<T> : Reply<S>
		{
			public readonly T Result;

			private Success (T result, Input<S> input, string found, IStream<string> expected) :
				base (input, found, expected)
			{
				Result = result;
			}
		}

		public class Failure : Reply<S>
		{
			private Failure (Input<S> input, string found, IStream<string> expected) :
				base (input, found, expected)
			{
			}
		}
	}
}
