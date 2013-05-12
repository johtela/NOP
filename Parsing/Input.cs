namespace NOP.Parsing
{
	using System;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	/// <summary>
	/// Generic interface for input streams with item type S.
	/// </summary>
	public interface IInput<S> : IStream<S>
	{
		object GetPosition ();
		new IInput<S> Rest { get; }
	}

	/// <summary>
	/// Some predifined inputs are defined here.
	/// </summary>
	public static class Input
	{
		public static void CheckNotEmpty<S> (this IInput<S> input)
		{
			if (input.IsEmpty)
				throw new ParseError ("Input is exhausted.");
		}

		/// <summary>
		/// Input for strings. Position is indicated by the index of current character.
		/// </summary>
		private struct StringPosInt : IInput<char>
		{
			private readonly string _str;
			private readonly int _pos;

			public StringPosInt (string str, int pos)
			{
				_str = str;
				_pos = pos;
			}

			public object GetPosition ()
			{
				return _pos;
			}

			public char First
			{
				get 
				{
					CheckNotEmpty (this);
					return _str[_pos]; 
				}
			}

			public IInput<char> Rest
			{
				get 
				{
					CheckNotEmpty (this);
					return new StringPosInt (_str, _pos + 1);
				}
			}

			IStream<char> IStream<char>.Rest { get { return Rest; } }
				 
			public bool IsEmpty
			{
				get { return _pos >= _str.Length; }
			}
		}

		/// <summary>
		/// Input stream for S-expressions. The current position is indicated
		/// by the path.
		/// </summary>
		private class SExprPosPath : IInput<SExpr>
		{
			private SExprPosPath _parent;
			private Sequence<SExpr> _seq;

			public SExprPosPath (SExprPosPath parent, Sequence<SExpr> seq)
			{
				_parent = parent;
				_seq = seq;
			}

			public object GetPosition ()
			{
				var path = Sequence<int>.Empty;
				for (var sp = this; sp._parent != null; sp = sp._parent)
					path += (sp._parent._seq.First as SExpr.List).Items.IndexOf(sp._seq.First);
				return new SExprPath (path);
			}

			public IInput<SExpr> Rest
			{
				get 
				{
					CheckNotEmpty (this);
					if (_seq.First is SExpr.List)
						return new SExprPosPath (this, (_seq.First as SExpr.List).Items);
					var sp = this;
					while (sp != null && sp._seq.RestL.IsEmpty)
						sp = sp._parent;
					return sp == null ?
						new SExprPosPath (null, Sequence<SExpr>.Empty) :
						new SExprPosPath (sp._parent, sp._seq.RestL);
				}
			}

			public SExpr First
			{
				get 
				{
					CheckNotEmpty (this);
					return _seq.First; 
				}
			}

			IStream<SExpr> IStream<SExpr>.Rest
			{
				get { return Rest; }
			}

			public bool IsEmpty
			{
				get { return _seq.IsEmpty; }
			}
		}

		/// <summary>
		/// Return an input stream for the given string.
		/// </summary>
		public static IInput<char> FromString (string str)
		{
			return new StringPosInt (str ?? string.Empty, 0);
		}

		public static IInput<SExpr> FromSExpr (SExpr root)
		{
			return new SExprPosPath (null, Sequence.Create (root));
		}
	}
}
