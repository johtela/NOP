namespace NOP.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Exception that is thrown if empty list is accessed.
	/// </summary>
	public class EmptyListException : Exception
	{
		public EmptyListException () : base("The list is empty") { }
	}

	/// <summary>
	/// An immutable linked list.
	/// </summary>
	/// <typeparam name="T">The item type of the list.</typeparam>
	public abstract class List<T> : IEnumerable<T>
	{
		public abstract T First { get; }

		public abstract List<T> Rest { get; protected set; }

		// Empty list.
		private sealed class EmptyList : List<T>
		{
			public override T First
			{
				get { throw new EmptyListException (); }
			}

			public override List<T> Rest
			{
				get { throw new EmptyListException (); }
				protected set { }
			}
		}

		// The singleton empty list.
		private static readonly EmptyList _empty = new EmptyList ();

		protected sealed class ConsList : List<T>
		{
			private T _first;
			private List<T> _rest;

			public ConsList (T first, List<T> rest)
			{
				_first = first;
				_rest = rest;
			}

			public override T First
			{
				get { return _first; }
			}

			public override List<T> Rest
			{
				get { return _rest; }
				protected set { _rest = value; }
			}
		}

		/// <summary>
		/// Create a list by appending an item at head of the list.
		/// </summary>
		/// <param name="head">The new head item.</param>
		/// <param name="tail">The tail of the list.</param>
		public static List<T> Cons (T first, List<T> rest)
		{
			return new ConsList (first, rest);
		}
		
		/// <summary>
		/// Construct a list from an enumerable.
		/// </summary>
		/// <param name='values'>The enumeration of values.</param>
		public static List<T> FromEnumerable (IEnumerable<T> values)
		{
			var result = Empty;
			var last = result;
			
			foreach (T item in values)
			{
				var cons = Cons (item, Empty);
				if (result.IsEmpty)
					result = cons;
				else
					last.Rest = cons;
				last = cons;
			}
			return result;
		}

		/// <summary>
		/// Return an empty list, i.e. null.
		/// </summary>
		public static List<T> Empty
		{
			get { return _empty; }
		}
		
		/// <summary>
		/// Is the list empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return this == Empty; }
		}

		/// <summary>
		/// Count the length, i.e. the number of items, in the list.
		/// </summary>
		public int Length
		{
			get
			{
				int result = 0;
				List<T > list = this;
				
				while (!list.IsEmpty)
				{
					result++;
					list = list.Rest;
				}
				return result;
			}
		}

		/// <summary>
		/// The last cons cell of the list. 
		/// </summary>
		public List<T> End
		{
			get
			{
				var result = this;
				while (!(result.IsEmpty || result.Rest.IsEmpty))
					result = result.Rest;
				return result;
			}
		}

		/// <summary>
		/// The last item in the list.
		/// </summary>
		public T Last
		{
			get { return End.First; }
		}

		/// <summary>
		/// Search for an item in the list.
		/// </summary>
		/// <param name="item">The item searched for.</param>
		/// <returns>The tail of the list from the point where the item is found, 
		/// or an empty list if the item is not found.</returns>
		public List<T> FindNext (T item)
		{
			List<T> list = this;
			
			while (!list.IsEmpty && !list.First.Equals (item))
				list = list.Rest;
			return list;
		}

		/// <summary>
		/// Find an item in the list that matches a predicate.
		/// </summary>
		/// <param name="predicate">The predicate that defines what to look for.</param>
		/// <returns>The tail of the list from the point where the predicate returned true, 
		/// or an empty list if none of the items matched the predicate.</returns>
		public List<T> FindNext (Predicate<T> predicate)
		{
			List<T > list = this;
			
			while (!list.IsEmpty && !predicate (list.First))
				list = list.Rest;
			return list;
		}

		/// <summary>
		/// Returns an item of the list that occurs in a given position.
		/// </summary>
		/// <param name="n">The position from the start of the item to be returned. 
		/// Zero returns the first element.</param>
		/// <returns>The item in the nth position.</returns>
		/// <exception cref="IndexOutOfRangeException">If n is negative the length of the list is less than n.
		/// </exception>
		public T Nth (int n)
		{
			List<T > list = this;
			
			while (!list.IsEmpty && n-- > 0)
				list = list.Rest;
			if (list.IsEmpty)
				throw new IndexOutOfRangeException ("The specified index is beyond the list end.");
			return list.First;
		}

		/// <summary>
		/// Copy the list upto the given element. 
		/// </summary>
		/// <param name="stop">The tail of the list that, when encountered, will 
		/// stop the copying. If that tail is not found, the entire list is copied.</param>
		/// <returns>The copied list upto the given tail; or the entire source
		/// list, if the tail is not found.</returns>
		public Tuple<List<T>, List<T>> CopyUpTo (List<T> stop)
		{
			List<T > list = this, last = Empty, first = Empty, prevLast;
			
			while (!list.IsEmpty && list != stop)
			{
				prevLast = last;
				last = new ConsList (list.First, Empty);
				prevLast.Rest = last;
				if (first.IsEmpty) first = last;
				list = list.Rest;
			}
			return new Tuple<List<T>, List<T>> (first, last);
		}

		/// <summary>
		/// Return a list with a new item inserted before the specified item.
		/// </summary>
		/// <param name="item">The item to be inserted.</param>
		/// <param name="before">The item before which the new item is inserted.</param>
		/// <returns>A new list with the item inserted before the specified item. If the before item
		/// is not found; the new item is added at the end of the list.</returns>
		public List<T> InsertBefore (T item, T before)
		{
			var tail = FindNext (before);
			
			return CopyUpTo (tail).Bind ((prefixFirst, prefixLast) =>
			{
				var cons = new ConsList (item, tail);
				prefixLast.Rest = cons;
				return prefixFirst.IsEmpty ? cons : prefixFirst;
			});
		}

		/// <summary>
		/// Remove an item from the list. 
		/// </summary>
		/// <param name="item">The item to be removed.</param>
		/// <returns>A new list without the given item.</returns>
		public List<T> Remove (T item)
		{
			var tail = FindNext (item);
			
			return CopyUpTo (tail).Bind ((prefixFirst, prefixLast) =>
			{
				var rest = tail.IsEmpty ? Empty : tail.Rest;
				prefixLast.Rest = rest;
				return prefixFirst.IsEmpty ? rest : prefixFirst;
			});
		}

		/// <summary>
		/// Reverses the list making the first item the last one, and vice versa. 
		/// </summary>
		/// <returns>This list in reverse order.</returns>
		public List<T> Reverse ()
		{
			var result = Empty;
			
			for (var list = this; !list.IsEmpty; list = list.Rest)
				result = Cons (list.First, result);
			return result;
		}

		/// <summary>
		/// Check if two lists are equal, that is contain the same items in
		/// the same order, and have equal lengths.
		/// </summary>
		/// <param name="obj">The other list that the list is compared with.</param>
		/// <returns>True, if the two lists contain the same items in the same order, 
		/// and have equal lengths.</returns>
		public bool EqualTo (List<T> other)
		{
			List<T > list = this;
			
			while (!list.IsEmpty && !other.IsEmpty && list.First.Equals (other.First))
			{
				list = list.Rest;
				other = other.Rest;
			}
			return list.IsEmpty && other.IsEmpty;
		}

		/// <summary>
		/// Collect the list of lists into another list.
		/// </summary>
		public List<U> Collect<U> (Func<T, List<U>> func)
		{
			var result = List<U>.Empty;
			var resultLast = result;
			
			for (var list = this; !list.IsEmpty; list = list.Rest)
			{
				var funcRes = func (list.First);
				if (list.Rest.IsEmpty)
				{
					if (result.IsEmpty)
						return funcRes;
					resultLast.Rest = funcRes;
					resultLast = funcRes.End;
				}
				else
					funcRes.CopyUpTo (null).Bind ((copyFirst, copyLast) =>
					{
						if (result.IsEmpty)
						{
							result = copyFirst;
							resultLast = copyLast;
						}
						else
						{
							resultLast.Rest = copyFirst;
							resultLast = copyLast;
						}
					});
			}
			return result;
		}

		/// <summary>
		/// Zip two lists together. 
		/// </summary>
		public List<Tuple<T, U>> ZipWith<U> (List<U> other)
		{
			var result = List<Tuple<T, U>>.Empty;
			var resultLast = result;
			
			for (var list = this; !(list.IsEmpty || other.IsEmpty); list = list.Rest,other = other.Rest)
			{
				var cons = new List<Tuple<T, U>>.ConsList (Tuple.Create (list.First, other.First), List<Tuple<T, U>>.Empty);
				if (result.IsEmpty)
				{
					result = cons;
					resultLast = cons;
				}
				else
				{
					resultLast.Rest = cons;
					resultLast = cons;
				}
			}
			return result;
		}

		/// <summary>
		/// Zip two lists together extending the shorter one with default values.
		/// </summary>
		public List<Tuple<T, U>> ZipExtendingWith<U> (List<U> other)
		{
			var result = List<Tuple<T, U>>.Empty;
			var resultLast = result;
			var list = this;
			
			while (!(list.IsEmpty && other.IsEmpty))
			{
				var listItem = default(T);
				var otherItem = default(U);
				
				if (!list.IsEmpty)
				{
					listItem = list.First;
					list = list.Rest;
				}
				if (!other.IsEmpty)
				{
					otherItem = other.First;
					other = other.Rest;
				}
				var cons = new List<Tuple<T, U>>.ConsList (Tuple.Create (listItem, otherItem), List<Tuple<T, U>>.Empty);
				if (result.IsEmpty)
				{
					result = cons;
					resultLast = cons;
				}
				else
				{
					resultLast.Rest = cons;
					resultLast = cons;
				}
			}
			return result;
		}

		/// <summary>
		/// Folds the list with a specified function and initial value.
		/// </summary>
		/// <param name='acc'>The initial value of the accumulator.</param>
		/// <param name='func'>The function to fold the list.</param>
		/// <typeparam name='U'>The type of the accumulator.</typeparam>
		public U Fold<U> (U acc, Func<U, T, U> func)
		{
			for (var list = this; !list.IsEmpty; list = list.Rest)
				acc = func (acc, list.First);
			return acc;
		}

		/// <summary>
		/// Folds the list backwards with a specified function and initial value.
		/// </summary>
		/// <param name='acc'>The initial value of the accumulator.</param>
		/// <param name='func'>The function to fold the list.</param>
		/// <typeparam name='U'>The type of the accumulator.</typeparam>
		public U FoldBack<U> (U acc, Func<U, T, U> func)
		{
			for (var list = Reverse (); !list.IsEmpty; list = list.Rest)
				acc = func (acc, list.First);
			return acc;
		}

		/// <summary>
		/// Map list to another list. 
		/// </summary>
		/// <param name="func">Function that maps the items.</param>
		/// <returns>The mapped list.</returns>
		public List<U> Map<U> (Func<T, U> func)
		{
			if (IsEmpty)
				return List<U>.Empty;
			
			var result = List.Cons (func (First));
			var last = result;
			
			for (var list = Rest; !list.IsEmpty; list = list.Rest, last = last.Rest)
				last.Rest = List.Cons (func (list.First));
			return result;
		}
		
		/// <summary>
		/// Determines whether the specified object is equal to the current list.
		/// </summary>
		/// <param name='obj'>
		/// The <see cref="System.Object"/> to compare with the current <see cref="NOP.Collections.List`1"/>.
		/// </param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="NOP.Collections.List`1"/>; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals (object obj)
		{
			var otherList = obj as List<T>;
			return (otherList != null) && EqualTo (otherList);
		}
		
		/// <summary>
		/// Serves as a hash function for a <see cref="NOP.Collections.List`1"/> object.
		/// </summary>
		/// <returns>
		/// A hash code for this instance that is suitable for use in hashing algorithms and data structures such 
		/// as a hash table.
		/// </returns>
		public override int GetHashCode ()
		{
			return Fold (0, (h, e) => h ^ e.GetHashCode ());
		}

		/// <summary>
		/// Returns a string representing the list.
		/// </summary>
		/// <typeparam name="T">The item type of the list,</typeparam>
		/// <param name="list">The list to be converted into string.</param>
		/// <returns>A string that lists the items in the list separated by comma
		/// and surrounded by square brackets. I.e. "[ item1, item2, ... ]"</returns>
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ("[");
			List<T > list = this;
			
			while (!list.IsEmpty)
			{
				sb.Append (list.First);
				list = list.Rest;
				
				if (list != Empty)
				{
					sb.Append (", ");
				}
			}
			sb.Append ("]");
			
			return sb.ToString ();
		}

		#region IEnumerable<T> Members

		/// <summary>
		/// Returns a new generic enumarator that can be used to iterate over the
		/// items in the list.
		/// </summary>
		/// <returns>An enumerator containing the items of the list.</returns>
		public IEnumerator<T> GetEnumerator ()
		{
			for (List<T> list = this; !list.IsEmpty; list = list.Rest)
				yield return list.First;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns a new non-generic enumarator that can be used to iterate over the
		/// items in the list.
		/// </summary>
		/// <returns>An enumerator containing the items of the list.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator ();
		}
		
		#endregion

		#region Operator overloads
		
		public static List<T> operator | (T first, List<T> rest)
		{
			return Cons (first, rest);
		}
		
		#endregion
	}
	
	/// <summary>
	/// Dynamic (non-generic) list of objects.
	/// </summary>
	public static class List
	{
		/// <summary>
		/// Helper to create a cons list without explicitly specifying the item type. 
		/// </summary>
		public static List<T> Cons<T> (T first, List<T> rest)
		{
			return List<T>.Cons (first, rest);
		}

		/// <summary>
		/// Helper to create a cons list without explicitly specifying the item type. 
		/// </summary>
		public static List<T> Cons<T> (T first)
		{
			return List<T>.Cons (first, List<T>.Empty);
		}

		/// <summary>
		/// Constructs a new list from an array.
		/// </summary>
		/// <param name="array">The array whose items are placed into the list.</param>
		/// <returns>A new list that contains the same items in the same order as the 
		/// array.</returns>
		public static List<T> FromArray<T> (T[] array)
		{
			List<T > result = List<T>.Empty;
			
			for (int i = array.Length - 1; i >= 0; i--)
				result = List<T>.Cons (array [i], result);
			return result;
		}

		/// <summary>
		/// Constructs a new list from a variable argument list.
		/// </summary>
		/// <param name="items">The items that are placed into the list.</param>
		/// <returns>A new list that contains the items in the order specified.</returns>
		public static List<T> Create<T> (params T[] items)
		{
			return FromArray (items);
		}

		/// <summary>
		/// Constructs a new list from IEnumerable.
		/// </summary>
		/// <param name="items">The items that are placed into the list.</param>
		/// <returns>A new list that contains the items in the order specified.</returns>
		public static List<T> Create<T> (IEnumerable<T> items)
		{
			return List<T>.FromEnumerable (items);
		}
	}
}
