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
		private struct StringInt : IInput<char>
		{
			private readonly string _str;
			private readonly int _pos;

			public StringInt (string str, int pos)
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
					return new StringInt (_str, _pos + 1);
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
		private class SExprSeqPath : IInput<Sequence<SExpr>>
		{
			private SExprSeqPath _parent;
			private Sequence<SExpr> _seq;

			public SExprSeqPath (SExprSeqPath parent, Sequence<SExpr> seq)
			{
				_parent = parent;
				_seq = seq;
			}

			public object GetPosition ()
			{
				var path = Sequence<int>.Empty;
				for (var sp = this; sp._parent != null; sp = sp._parent)
				{
					var pseq = (sp._parent._seq.First as SExpr.List).Items;
					path += sp._seq.IsEmpty ? pseq.Length : pseq.IndexOf (sp._seq.First);
				}
				return new SExprPath (path);
			}

			public Sequence<SExpr> First
			{
				get
				{
					CheckNotEmpty (this);
					return _seq;
				}
			}

			public IInput<Sequence<SExpr>> Rest
			{
				get 
				{
					CheckNotEmpty (this);
					if (_seq.IsEmpty)
						return new SExprSeqPath (_parent._parent, _parent._seq.RestL);
					else if (_seq.First is SExpr.List)
						return new SExprSeqPath (this, (_seq.First as SExpr.List).Items);
					else
						return new SExprSeqPath (_parent, _seq.RestL);
				}
			}

			IStream<Sequence<SExpr>> IStream<Sequence<SExpr>>.Rest
			{
				get { return Rest; }
			}

			public bool IsEmpty
			{
				get { return _seq.IsEmpty && _parent == null; }
			}
		}

		/// <summary>
		/// Return an input stream for the given string.
		/// </summary>
		public static IInput<char> FromString (string str)
		{
			return new StringInt (str ?? string.Empty, 0);
		}

		/// <summary>
		/// Return an input stream for the given S-expression. The expression tree
		/// is traversed in a depth first manner.
		/// </summary>
		public static IInput<Sequence<SExpr>> FromSExpr (SExpr root)
		{
			return new SExprSeqPath (null, Sequence.Create (root));
		}
	}
}
