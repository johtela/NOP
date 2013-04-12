﻿namespace NOP.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using NOP.Visuals;

	/// <summary>
	/// Random access sequence built on top of finger tree.
	/// </summary>
	/// <typeparam name="T">The item type of the sequence.</typeparam>
	public struct Sequence<T> : IReducible<T>, IEnumerable<T>, IVisualizable
	{
		/// <summary>
		/// The size monoid defines how tree sizes are accumulated.
		/// The size is just an integer and the plus operation is
		/// the normal adition.
		/// </summary>
		public struct Size : IMonoid<Size>
		{
			private int _value;

			public Size (int value)
			{
				_value = value;
			}

			public static implicit operator int (Size ts)
			{
				return ts._value;
			}

			public Size Plus (Size other)
			{
				return new Size (_value + other._value);
			}

			public override string ToString ()
			{
				return _value.ToString ();
			}
		}

		/// <summary>
		/// The wrapper structure for elemens of the sequence.
		/// This is needed to implement the IMeasurable interface.
		/// </summary>
		public struct Elem : IMeasurable<Size>
		{
			public readonly T Value;

			public Elem (T value)
			{
				Value = value;
			}

			public static implicit operator T (Elem e)
			{
				return e.Value;
			}

			public Size Measure ()
			{
				return new Size (1);
			}

			public override bool Equals (object obj)
			{
				return obj is Elem && Value.Equals (((Elem)obj).Value);
			}

			public override int GetHashCode ()
			{
				return Value.GetHashCode ();
			}

			public override string ToString ()
			{
				return Value.ToString ();
			}
		}

		// The finger tree wrapped in the Sequence.
		private FingerTree<Elem, Size> _tree;

		private Sequence (FingerTree<Elem, Size> tree)
		{
			_tree = tree;
		}

		private static Elem CreateElem (T item)
		{
			return new Elem (item);
		}

		private static Func<Size, bool> FindP(int index)
		{
			return sz => index < sz;
		}

		public static Sequence<T> FromEnumerable (IEnumerable<T> items)
		{
			return new Sequence<T> (FingerTree<Elem, Size>.FromEnumerable (items.Select (CreateElem)));
		}

		public static Sequence<T> Empty
		{
			get { return new Sequence<T> (FingerTree<Elem, Size>.Empty); }
		}

		public bool IsEmpty
		{
			get { return _tree.IsEmpty; }
		}

		public T First
		{
			get { return _tree.First; }
		}

		public T Last
		{
			get { return _tree.Last; }
		}

		public Sequence<T> RestL
		{
			get { return new Sequence<T> (_tree.RestL); }
		}

		public Sequence<T> RestR
		{
			get { return new Sequence<T> (_tree.RestR); }
		}

		public Tuple<T, Sequence<T>> LeftView
		{
			get 
			{
				var view = _tree.LeftView ();
				return view == null ? null : 
					Tuple.Create ((T)view.First, new Sequence<T> (view.Rest));
			}
		}

		public Tuple<Sequence<T>, T> RightView
		{
			get
			{
				var view = _tree.RightView ();
				return view == null ? null : 
					Tuple.Create (new Sequence<T> (view.Rest), (T)view.Last);
			}
		}

		public T this[int index]
		{
			get 
			{
				if (index < 0 || index >= Length)
					throw new IndexOutOfRangeException ("Index was outside the bounds of the sequence.");
				return _tree.Split (FindP (index), new Size ()).Item;
			}
		}

		public int Length
		{
			get { return _tree.Measure (); }
		}

		public Sequence<T> AppendWith (NOPList<T> items, Sequence<T> other)
		{
			return new Sequence<T> (_tree.AppendTree (items.Map (CreateElem), other._tree));
		}

		public Tuple<Sequence<T>, T, Sequence<T>> SplitAt (int index)
		{
			var split = _tree.Split (FindP (index), new Size ());
			return Tuple.Create (new Sequence<T> (split.Left), (T)split.Item, 
				new Sequence<T> (split.Right));
		}

		public T[] AsArray ()
		{
			var result = new T[Length];
			_tree.ReduceLeft (0, (i, e) =>
			{
				result[i] = e;
				return i + 1;
			});
			return result;
		}

		public Sequence<U> Map<U> (Func<T, U> map)
		{
			return new Sequence<U> (_tree.ReduceLeft (FingerTree<Sequence<U>.Elem, Sequence<U>.Size>.Empty,
				(t, e) => t.AddRight (Sequence<U>.CreateElem (map (e)))));
		}

		#region Overridden from Object

		public override bool Equals (object obj)
		{
			if (!(obj is Sequence<T>)) return false;
			var t1 = _tree;
			var t2 = ((Sequence<T>)obj)._tree;

			while (!(t1.IsEmpty || t2.IsEmpty))
			{
				if (!t1.First.Equals (t2.First)) return false;
				t1 = t1.RestL;
				t2 = t2.RestL;
			}
			return t1.IsEmpty && t2.IsEmpty;
		}

		public override int GetHashCode ()
		{
			return ReduceLeft (0, (h, e) => h ^ e.GetHashCode ());
		}

		public override string ToString ()
		{
			return ToString ("[", "]", ", ");
		}

		public string ToString (string openBracket, string closeBracket, string separator)
		{
			var sb = new StringBuilder (openBracket);
			if (Length > 0)
			{
				sb.Append (First);
				RestL.Foreach (e => sb.AppendFormat ("{0}{1}", separator, e));
			}
			sb.Append (closeBracket);
			return sb.ToString ();
		}

		#endregion

		#region Operator overloads

		public static Sequence<T> operator + (T item, Sequence<T> seq)
		{
			return new Sequence<T> (seq._tree.AddLeft (CreateElem (item)));
		}

		public static Sequence<T> operator + (Sequence<T> seq, T item)
		{
			return new Sequence<T> (seq._tree.AddRight (CreateElem (item)));
		}

		public static Sequence<T> operator + (Sequence<T> seq, Sequence<T> other)
		{
			return new Sequence<T> (seq._tree.AppendTree (NOPList<Elem>.Empty, other._tree));
		}

		#endregion

		#region IReducible<T> implementation

		public U ReduceLeft<U> (U acc, Func<U, T, U> func)
		{
			return _tree.ReduceLeft (acc, (a, e) => func (a, e.Value));
		}

		public U ReduceRight<U> (Func<T, U, U> func, U acc)
		{
			return _tree.ReduceRight ((e, a) => func (e.Value, a), acc);
		}

		#endregion

		#region IEnumerable<T> implementation

		public IEnumerator<T> GetEnumerator ()
		{
			return _tree.ReduceRight ((e, l) => List.Cons (e, l), NOPList<T>.Empty)
				.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		#endregion

		#region IVisualizable implementation
	
		public Visual ToVisual ()
		{
			return _tree.ToVisual ();
		}

		#endregion
	}

	/// <summary>
	/// Static helper class for creating sequences.
	/// </summary>
	public static class Sequence
	{
		public static Sequence<T> Create<T> (IEnumerable<T> items)
		{
			return Sequence<T>.FromEnumerable (items);
		}

		public static Sequence<T> Create<T> (params T[] items)
		{
			return Sequence<T>.FromEnumerable (items);
		}
	}
}