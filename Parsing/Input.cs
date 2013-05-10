namespace NOP.Parsing
{
	using System;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	public interface IInput<S> : IStream<S>
	{
		object GetPosition ();
		new IInput<S> Rest { get; }
	}

	public static class Input
	{
		private struct StringInt : IInput<char>
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

			public object GetPosition ()
			{
				return _pos;
			}

			public char First
			{
				get 
				{
					CheckNotEmpty ();
					return _str[_pos]; 
				}
			}

			public IInput<char> Rest
			{
				get 
				{
					CheckNotEmpty ();
					return new StringInt (_str, _pos + 1);
				}
			}

			IStream<char> IStream<char>.Rest { get { return Rest; } }
				 
			public bool IsEmpty
			{
				get { return _pos >= _str.Length; }
			}
		}

		public static IInput<char> FromString (string str)
		{
			return new StringInt (str ?? string.Empty, 0);
		}
	}
}
