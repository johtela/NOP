namespace NOP.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Tree empty exception.
	/// </summary>
	public class EmptyTreeException : Exception
	{
		public EmptyTreeException () : base ("Tree is empty.") { }
	}

	/// <summary>
	/// The inner node of the finger tree has either degree two or three.
	/// </summary>
	public abstract class Node<T>
	{
		public abstract Digit<T> ToDigit ();

		private sealed class Node2 : Node<T>
		{
			public readonly T Item1;
			public readonly T Item2;

			public Node2 (T item1, T item2)
			{
				Item1 = item1;
				Item2 = item2;
			}

			public override Digit<T> ToDigit ()
			{
				return new Digit<T> (Item1, Item2);
			}
		}

		private sealed class Node3 : Node<T>
		{
			public readonly T Item1;
			public readonly T Item2;
			public readonly T Item3;

			public Node3 (T item1, T item2, T item3)
			{
				Item1 = item1;
				Item2 = item2;
				Item3 = item3;
			}

			public override Digit<T> ToDigit ()
			{
				return new Digit<T> (Item1, Item2, Item3);
			}
		}

		public static Node<T> Create (T item1, T item2)
		{
			return new Node2 (item1, item2);
		}

		public static Node<T> Create (T item1, T item2, T item3)
		{
			return new Node3 (item1, item2, item3);
		}

		public static NOPList<Node<T>> CreateMany (NOPList<T> items)
		{
			switch (items.Length)
			{
				case 0:
				case 1: throw new ArgumentException ("List should contain at least two items.");
				case 2: return List.Cons (Create (items.First, items.Rest.First));
				case 3: return List.Cons (Create (items.First, items.Rest.First, items.Rest.Rest.First));
				case 4: return List.Create (Create (items.First, items.Rest.First), 
					Create (items.Rest.Rest.First, items.Rest.Rest.Rest.First));
				default: return Create (items.First, items.Rest.First, items.Rest.Rest.First) |
					CreateMany (items.Rest.Rest.Rest);
			}
		}
	}

	/// <summary>
	/// The front and back parts of the tree have one to four items in an array.
	/// </summary>
	public class Digit<T> : IEnumerable<T>
	{
		private readonly T[] _items;

		public Digit (T[] items)
		{
			var len = items.Length;
			if (len < 1 || len > 4)
				throw new ArgumentException ("Digit array must have length of 1..4");
			_items = items;
		}

		public Digit (T item1)
		{
			_items = new T[] { item1 };
		}

		public Digit (T item1, T item2)
		{
			_items = new T[] { item1, item2 };
		}

		public Digit (T item1, T item2, T item3)
		{
			_items = new T[] { item1, item2, item3 };
		}

		public Digit (T item1, T item2, T item3, T item4)
		{
			_items = new T[] { item1, item2, item3, item4 };
		}

		public static Digit<T> operator+ (T item, Digit<T> digit)
		{
			switch (digit._items.Length)
			{
				case 1: return new Digit<T> (item, digit._items[0]);
				case 2: return new Digit<T> (item, digit._items[0], digit._items[1]);
				case 3: return new Digit<T> (item, digit._items[0], digit._items[1], digit._items[2]);
				default: throw new ArgumentException ("Digit is full", "digit");
			}
		}

		public static Digit<T> operator + (Digit<T> digit, T item)
		{
			switch (digit._items.Length)
			{
				case 1: return new Digit<T> (digit._items[0], item);
				case 2: return new Digit<T> (digit._items[0], digit._items[1], item);
				case 3: return new Digit<T> (digit._items[0], digit._items[1], digit._items[2], item);
				default: throw new ArgumentException ("Digit is full", "digit");
			}
		}

		public T this[int index]
		{
			get { return _items[index]; }
		}

		public bool IsFull
		{
			get { return _items.Length > 3; }
		}

		public T First
		{
			get { return _items[0]; }
		}

		public T Last
		{
			get { return _items[_items.Length - 1]; }
		}

		public T[] Suffix
		{
			get 
			{ 
				var len = _items.Length;
				if (len == 1)
					return null;
				var result = new T[len - 1];
				_items.CopyTo (result, 1);
				return result;
			}
		}

		public T[] Prefix
		{
			get
			{
				var len = _items.Length;
				if (len == 1)
					return null;
				var result = new T[len - 1];
				_items.CopyTo (result, 0);
				return result;
			}
		}

		#region IEnumerable implementation
		
		public IEnumerator<T> GetEnumerator ()
		{
			return (_items as IEnumerable<T>).GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _items.GetEnumerator ();
		}

		#endregion	
	}

	/// <summary>
	/// Left view of the tree.
	/// </summary>
	public class ViewL<T>
	{
		public readonly T First;
		public readonly FingerTree<T> Rest;

		public ViewL (T first, FingerTree<T> rest)
		{
			First = first;
			Rest = rest;
		}
	}

	/// <summary>
	/// Rigth view of the tree.
	/// </summary>
	public class ViewR<T>
	{
		public readonly T Last;
		public readonly FingerTree<T> Rest;

		public ViewR (T last, FingerTree<T> rest)
		{
			Last = last;
			Rest = rest;
		}
	}

	/// <summary>
	/// The finger tree is either empty, contains a single item, or then it contains
	/// front, inner, and back parts where the inner part can be empty.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class FingerTree<T>
	{
		protected abstract FingerTree<T> AddLeft (T leftItem);
		protected abstract FingerTree<T> AddRight (T rightItem);

		protected abstract ViewL<T> LeftView ();
		protected abstract ViewR<T> RightView ();

		protected abstract FingerTree<T> AppendTree (IEnumerable<T> items, FingerTree<T> tree);

		private static FingerTree<T> _empty = new Empty ();

		/// <summary>
		/// An empty tree.
		/// </summary>
		private sealed class Empty : FingerTree<T>
		{
			public Empty () {}

			protected override FingerTree<T> AddLeft (T leftItem)
			{
				return new Single (leftItem);
			}

			protected override FingerTree<T> AddRight (T rightItem)
			{
				return new Single (rightItem);
			}

			protected override ViewL<T> LeftView ()
			{
				return null;
			}

			protected override ViewR<T> RightView ()
			{
				return null;
			}

			protected override FingerTree<T> AppendTree (IEnumerable<T> items, FingerTree<T> tree)
			{
				return tree.Prepend (items);
			}
		}

		/// <summary>
		/// Tree with a single item.
		/// </summary>
		private sealed class Single : FingerTree<T>
		{
			public readonly T Item;

			public Single (T item)
			{
				Item = item;
			}

			protected override FingerTree<T> AddLeft (T leftItem)
			{
				return new Deep (new Digit<T> (leftItem), 
					new FingerTree<Node<T>>.Empty (), 
					new Digit<T> (Item));
			}

			protected override FingerTree<T> AddRight (T rightItem)
			{
				return new Deep (new Digit<T> (Item),
					new FingerTree<Node<T>>.Empty (),
					new Digit<T> (rightItem));
			}

			protected override ViewL<T> LeftView ()
			{
				return new ViewL<T> (Item, _empty);
			}

			protected override ViewR<T> RightView ()
			{
				return new ViewR<T> (Item, _empty);
			}

			protected override FingerTree<T> AppendTree (IEnumerable<T> items, FingerTree<T> tree)
			{
				return tree.Prepend (items).AddLeft (Item);
			}
		}

		/// <summary>
		/// Deep tree with a front and back digits plus the inner tree.
		/// </summary>
		private sealed class Deep : FingerTree<T>
		{
			public readonly Digit<T> Front;
			public readonly FingerTree<Node<T>> Inner;
			public readonly Digit<T> Back;

			public Deep (Digit<T> front, FingerTree<Node<T>> inner, Digit<T> back)
			{
				Front = front;
				Inner = inner;
				Back = back;
			}

			protected override FingerTree<T> AddLeft (T leftItem)
			{
				return Front.IsFull ?
					new Deep (new Digit<T> (leftItem, Front[0]),
						Inner.AddLeft (Node<T>.Create (Front[1], Front[2], Front[3])),
						Back) :
					new Deep (leftItem + Front, Inner, Back);
			}

			protected override FingerTree<T> AddRight (T rightItem)
			{
				return Back.IsFull ?
					new Deep (Front,
						Inner.AddRight (Node<T>.Create (Back[0], Back[1], Back[2])),
						new Digit<T> (Back[3], rightItem)) :
					new Deep (Front, Inner, Back + rightItem);
			}

			protected override ViewL<T> LeftView ()
			{
				return new ViewL<T> (Front.First, DeepL (Front.Suffix, Inner, Back));
			}

			protected override ViewR<T> RightView ()
			{
				return new ViewR<T> (Back.Last, DeepR (Front, Inner, Back.Prefix));
			}

			protected override FingerTree<T> AppendTree (IEnumerable<T> items, FingerTree<T> tree)
			{
				if (tree is Empty)
					return Append (items);
				if (tree is Single)
					return Append (items).AddRight ((tree as Single).Item);
				var other = tree as Deep;
				var innerItems = List.Create (Back.Concat (items).Concat (other.Front));
				return new Deep (Front,
					Inner.AppendTree (Node<T>.CreateMany (innerItems), other.Inner),
					other.Back);
			}
		}

		public bool IsEmpty
		{
			get { return this is Empty; }
		}

		public T First
		{
			get { return CheckView (LeftView ()).First; }
		}

		public T Last
		{
			get { return CheckView (RightView ()).Last; }
		}

		public FingerTree<T> RestL
		{
			get { return CheckView (LeftView ()).Rest; }
		}

		public FingerTree<T> RestR
		{
			get { return CheckView (RightView ()).Rest; }
		}

		public FingerTree<T> Prepend (IEnumerable<T> items)
		{
			var result = this;
			foreach (T item in items.Reverse ())
				result = result.AddLeft (item);
			return result;
		}

		public FingerTree<T> Append (IEnumerable<T> items)
		{
			var result = this;
			foreach (T item in items)
				result = result.AddRight (item);
			return result;
		}

		public static FingerTree<T> FromEnumerable (IEnumerable<T> e)
		{
			return _empty.Append (e);
		}

		private ViewL<T> CheckView (ViewL<T> viewl)
		{
			if (viewl == null)
				throw new EmptyTreeException ();
			return viewl;
		}

		private ViewR<T> CheckView (ViewR<T> viewr)
		{
			if (viewr == null)
				throw new EmptyTreeException ();
			return viewr;
		}

		private static FingerTree<T> DeepL (T[] front, FingerTree<Node<T>> inner, Digit<T> back)
		{
			if (front == null)
			{
				var viewl = inner.LeftView ();
				return viewl == null ?
					FromEnumerable (back) :
					new Deep (viewl.First.ToDigit (), viewl.Rest, back);
			}
			return new Deep (new Digit<T> (front), inner, back);
		}

		private static FingerTree<T> DeepR (Digit<T> front, FingerTree<Node<T>> inner, T[] back)
		{
			if (back == null)
			{
				var viewr = inner.RightView ();
				return viewr == null ?
					FromEnumerable (front) :
					new Deep (front, viewr.Rest, viewr.Last.ToDigit ());
			}
			return new Deep (front, inner, new Digit<T> (back));
		}
	}
}
