namespace NOP.Parsing
{
	using System;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	public abstract class Input<S, P> : IStream<S>
	{
		public readonly P Position;

		public Input (P position)
		{
			Position = position;
		}

		public abstract S First { get; }

		public abstract Input<S, P> Rest { get; }

		public abstract bool IsEmpty { get; }

		IStream<S> IStream<S>.Rest { get { return Rest; } }
	}
}
