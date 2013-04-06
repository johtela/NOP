namespace NOP.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public struct Sequence<T>
	{
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
		}

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
		}

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

		public static Sequence<T> Create (IEnumerable<T> items)
		{
			return new Sequence<T> (FingerTree<Elem, Size>.FromEnumerable (items.Select(CreateElem)));
		}

		public static Sequence<T> Create (params T[] items)
		{
			return new Sequence<T> (FingerTree<Elem, Size>.FromEnumerable (items.Select(CreateElem)));
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
				return Tuple.Create ((T)view.First, new Sequence<T> (view.Rest));
			}
		}

		public Tuple<Sequence<T>, T> RightView
		{
			get
			{
				var view = _tree.RightView ();
				return Tuple.Create (new Sequence<T> (view.Rest), (T)view.Last);
			}
		}

		public T this[int index]
		{
			get { return _tree.Split (FindP (index), new Size ()).Item; }
		}

		public Sequence<T> AppendWith (IEnumerable<T> items, Sequence<T> other)
		{
			return new Sequence<T> (
				_tree.AppendTree (items.Select (CreateElem), other._tree));
		}

		public Tuple<Sequence<T>, T, Sequence<T>> SplitAt (int index)
		{
			var split = _tree.Split (FindP (index), new Size ());
			return Tuple.Create (new Sequence<T> (split.Left), (T)split.Item, 
				new Sequence<T> (split.Right));
		}

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
	}
}
