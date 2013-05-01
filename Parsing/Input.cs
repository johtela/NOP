namespace NOP.Parsing
{
	using System;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	public abstract class Input<S> : IStream<S>
	{
		public abstract P GetPosition<P> ();

		public abstract S First { get; }

		public abstract Input<S> Rest { get; }

		public abstract bool IsEmpty { get; }

		IStream<S> IStream<S>.Rest { get { return Rest; } }
	}
}
