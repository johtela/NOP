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
		public readonly IStream<string> Expected;

		public class Success : Reply<T, S, P>
		{
			public readonly T Value;

			public Success (T result, Input<S, P> input, string found, IStream<string> expected)
			{

			}
		}
	}
}
