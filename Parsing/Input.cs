namespace NOP.Parsing
{
	using System;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	public abstract class Input<S, P> : IStream<S>
	{
		public abstract P GetPosition ();

		public abstract S First { get; }

		public abstract Input<S, P> Rest { get; }

		public abstract bool IsEmpty { get; }

		IStream<S> IStream<S>.Rest { get { return Rest; } }
	}

	public static class Input
	{
		private class StringInt : Input<char, int>
		{
			private readonly string _str;
			private readonly int _pos;

			public StringInt (string str, int pos)
			{
				_str = str;
				_pos = pos;
			}

			private void CheckNotEmpty ()
			{
				if (IsEmpty)
					throw new ParseError ("String is exhausted.");
			}

			public override int GetPosition ()
			{
				return _pos;
			}

			public override char First
			{
				get 
				{
					CheckNotEmpty ();
					return _str[_pos]; 
				}
			}

			public override Input<char, int> Rest
			{
				get 
				{
					CheckNotEmpty ();
					return new StringInt (_str, _pos + 1);
				}
			}

			public override bool IsEmpty
			{
				get { return _pos >= _str.Length; }
			}
		}

		public static Input<char, int> FromString (string str)
		{
			return new StringInt (str ?? string.Empty, 0);
		}
	}
}
