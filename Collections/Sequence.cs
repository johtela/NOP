namespace NOP.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public struct Sequence<T> : IReducible<T>
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

			public override string ToString ()
			{
				return _value.ToString ();
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

			public override string ToString ()
			{
				return Value.ToString ();
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
	}
}
