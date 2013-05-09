namespace NOP.Parsing
{
	using System;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	public abstract class Input<S> : IStream<S>
	{
		public abstract object GetPosition ();
		
		public abstract S First { get; }

		public abstract Input<S> Rest { get; }

		public abstract bool IsEmpty { get; }

		IStream<S> IStream<S>.Rest { get { return Rest; } }
	}

	public static class Input
	{
		private class StringInt : Input<char>
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

			public override object GetPosition ()
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

			public override Input<char> Rest
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

		public static Input<char> FromString (string str)
		{
			return new StringInt (str ?? string.Empty, 0);
		}
	}
}
