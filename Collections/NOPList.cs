namespace NOP.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using NOP.Visuals;

	/// <summary>
	/// Exception that is thrown if empty list is accessed.
	/// </summary>
	public class EmptyListException : Exception
	{
		public EmptyListException () : base ("The list is empty") { }
	}

	/// <summary>
	/// An immutable linked list.
	/// </summary>
	/// <typeparam name="T">The item type of the list.</typeparam>
	public class NOPList<T> : IEnumerable<T>, IVisualizable
	{
		private static readonly NOPList<T> _empty = new NOPList<T> (default(T), null);
		private T _first;
		private NOPList<T> _rest;
		
		/// <summary>
		/// The first item in the list.
		/// </summary>
		public T First
		{ 
			get
			{
				if (this == _empty)
					throw new EmptyListException ();
				return _first;
			}
		}
			
		/// <summary>
		/// The rest of the list.
		/// </summary>
		public NOPList<T> Rest
		{ 
			get
			{
				if (this == _empty)
					throw new EmptyListException ();
				return _rest;
			}
			private set
			{
				if (this != _empty)
					_rest = value;
			}
		}
		
		private NOPList (T first, NOPList<T> rest)
		{
			_first = first;
			_rest = rest;
		}

		/// <summary>
		/// Create a list by appending an item at head of the list.
		/// </summary>
		/// <param name="head">The new head item.</param>
		/// <param name="tail">The tail of the list.</param>
		public static NOPList<T> Cons (T first, NOPList<T> rest)
		{
			return new NOPList<T> (first, rest);
		}
		
		/// <summary>
		/// Construct a list from an enumerable.
		/// </summary>
		/// <param name='values'>The enumeration of values.</param>
		public static NOPList<T> FromEnumerable (IEnumerable<T> values)
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
		public static NOPList<T> Empty
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
		/// Is the list non-empty.
		/// </summary>
		public bool NotEmpty
		{
			get { return this != Empty; }
		}

		/// <summary>
		/// Count the length, i.e. the number of items, in the list.
		/// </summary>
		public int Length
		{
			get
			{
				int result = 0;
				NOPList<T > list = this;
				
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
		public NOPList<T> End
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
		public NOPList<T> FindNext (T item)
		{
			NOPList<T> list = this;
			
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
		public NOPList<T> FindNext (Predicate<T> predicate)
		{
			NOPList<T> list = this;
			
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
			var list = this;
			
			while (!list.IsEmpty && n-- > 0)
				list = list.Rest;
			if (list.IsEmpty)
				throw new IndexOutOfRangeException ("The specified index is beyond the list end.");
			return list.First;
		}

		/// <summary>
		/// Returns an item of the list that occurs in a given position
		/// or default value if the position is beyond the end of the list.
		/// </summary>
		/// <param name="n">The position from the start of the item to be returned. 
		/// Zero returns the first element.</param>
		/// <returns>The item in the nth position, or default(T) if the position
		/// is beyond list end.</returns>
		public T NthOrDefault (int n)
		{
			var list = this;

			while (!list.IsEmpty && n-- > 0)
				list = list.Rest;
			return list.IsEmpty ? default (T) : list.First;
		}

		/// <summary>
		/// Return the Nth tail of the list.
		/// </summary>
		/// <param name="n">The index of the tail to be returned.</param>
		/// <returns>The nth tail of the list.</returns>
		public NOPList<T> RestNth (int n)
		{
			var list = this;

			while (n-- > 0)
				list = list.Rest;
			return list;
		}

		/// <summary>
		/// Return the position of the specified item.
		/// </summary>
		/// <param name="item">The item to be searched for.</param>
		/// <returns>The zero-based position of the item, or -1, if
		/// the item was not found.</returns>
		public int IndexOf (T item)
		{
			var i = 0;
			var list = this;

			while (!list.IsEmpty)
			{
				if (list.First.Equals (item)) 
					return i;
				list = list.Rest;
				i++;
			}
			return -1;
		}

		/// <summary>
		/// Copy the list upto the given element. 
		/// </summary>
		/// <param name="stop">The tail of the list that, when encountered, will 
		/// stop the copying. If that tail is not found, the entire list is copied.</param>
		/// <returns>The copied list upto the given tail; or the entire source
		/// list, if the tail is not found.</returns>
		public Tuple<NOPList<T>, NOPList<T>> CopyUpTo (NOPList<T> stop)
		{
			NOPList<T> list = this, last = Empty, first = Empty, prevLast;
			
			while (!list.IsEmpty && list != stop)
			{
				prevLast = last;
				last = Cons (list.First, Empty);
				prevLast.Rest = last;
				if (first.IsEmpty)
					first = last;
				list = list.Rest;
			}
			return new Tuple<NOPList<T>, NOPList<T>> (first, last);
		}

		/// <summary>
		/// Append an item at the end of the list.
		/// </summary>
		/// <param name="item">Item to be appended.</param>
		/// <returns>A new list with <paramref name="item"/> as its last element.</returns>
		public NOPList<T> Append(T item)
		{
			return CopyUpTo (Empty).Bind ((prefixFirst, prefixLast) =>
			{
				var cons = Cons (item, Empty);
				prefixLast.Rest = cons;
				return prefixFirst.IsEmpty ? cons : prefixFirst;
			});
		}

		/// <summary>
		/// Return a list with a new item inserted before the specified item.
		/// </summary>
		/// <param name="item">The item to be inserted.</param>
		/// <param name="before">The item before which the new item is inserted.</param>
		/// <returns>A new list with the item inserted before the specified item. If the before item
		/// is not found; the new item is added at the end of the list.</returns>
		public NOPList<T> InsertBefore (T item, T before)
		{
			var tail = FindNext (before);
			
			return CopyUpTo (tail).Bind ((prefixFirst, prefixLast) =>
			{
				var cons = Cons (item, tail);
				prefixLast.Rest = cons;
				return prefixFirst.IsEmpty ? cons : prefixFirst;
			});
		}

		/// <summary>
		/// Remove an item from the list. 
		/// </summary>
		/// <param name="item">The item to be removed.</param>
		/// <returns>A new list without the given item.</returns>
		public NOPList<T> Remove (T item)
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
		public NOPList<T> Reverse ()
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
		/// <param name="other">The other list that the list is compared with.</param>
		/// <param name="equals">The function that compares if two items are equal.</param>
		/// <returns>True, if the two lists contain the same items in the same order, 
		/// and have equal lengths.</returns>
		public bool EqualTo (NOPList<T> other, Func<T, T, bool> equals)
		{
			NOPList<T > list = this;
			
			while (!list.IsEmpty && !other.IsEmpty && equals (list.First, other.First))
			{
				list = list.Rest;
				other = other.Rest;
			}
			return list.IsEmpty && other.IsEmpty;
		}
		
		/// <summary>
		/// Check if two lists are equal, that is contain the same items in
		/// the same order, and have equal lengths.
		/// </summary>
		/// <param name="other">The other list that the list is compared with.</param>
		/// <returns>True, if the two lists contain the same items in the same order, 
		/// and have equal lengths.</returns>
		public bool EqualTo (NOPList<T> other)
		{
			return this.EqualTo (other, (i1, i2) => i1.Equals (i2));
		}	

		/// <summary>
		/// Collect the list of lists into another list.
		/// </summary>
		public NOPList<U> Collect<U> (Func<T, NOPList<U>> func)
		{
			var result = NOPList<U>.Empty;
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
		public NOPList<Tuple<T, U>> ZipWith<U> (NOPList<U> other)
		{
			var result = NOPList<Tuple<T, U>>.Empty;
			var resultLast = result;
			
			for (var list = this; !(list.IsEmpty || other.IsEmpty); list = list.Rest,other = other.Rest)
			{
				var cons = NOPList<Tuple<T, U>>.Cons (Tuple.Create (list.First, other.First), NOPList<Tuple<T, U>>.Empty);
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
		public NOPList<Tuple<T, U>> ZipExtendingWith<U> (NOPList<U> other)
		{
			var result = NOPList<Tuple<T, U>>.Empty;
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
				var cons = NOPList<Tuple<T, U>>.Cons (Tuple.Create (listItem, otherItem), NOPList<Tuple<T, U>>.Empty);
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
		/// Folds the list with other list. The fold is terminated when
		/// either of the lists are exhausted.
		/// </summary>
		/// <returns>The accumulated value.</returns>
		/// <param name='acc'>Initial accumulator value.</param>
		/// <param name='func'>The function applied to the lists' items.</param>
		/// <param name='other'>The list to be folded with.</param>
		public U FoldWith<U, V> (U acc, Func<U, T, V, U> func, NOPList<V> other)
		{
			for (var list = this; !(list.IsEmpty || other.IsEmpty); 
				 list = list.Rest, other = other.Rest)
				acc = func (acc, list.First, other.First);
			return acc;
		}

		/// <summary>
		/// Map list to another list. 
		/// </summary>
		/// <param name="func">Function that maps the items.</param>
		/// <returns>The mapped list.</returns>
		public NOPList<U> Map<U> (Func<T, U> func)
		{
			if (IsEmpty)
				return NOPList<U>.Empty;
			
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
			var otherList = obj as NOPList<T>;
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
		public override string ToString ()
		{
			return ToString ("[", "]", ", ");
		}
		
		/// <summary>
		/// Returns a string representing the list. Gets open, close bracket, and separtor as
		/// an argument.
		/// </summary>
		public string ToString (string openBracket, string closeBracket, string separator)
		{
			StringBuilder sb = new StringBuilder (openBracket);
			NOPList<T > list = this;
			
			while (!list.IsEmpty)
			{
				sb.Append (list.First);
				list = list.Rest;
				
				if (list != Empty)
					sb.Append (separator);
			}
			sb.Append (closeBracket);
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
			for (NOPList<T> list = this; !list.IsEmpty; list = list.Rest)
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

		#region IVisualizable members

		public Visual ToVisual ()
		{
			throw new NotImplementedException ();
		}

		#endregion

		#region Operator overloads

		public static NOPList<T> operator | (T first, NOPList<T> rest)
		{
			return Cons (first, rest);
		}

		#endregion
	}
	
	/// <summary>
	/// Static helper class for creating lists.
	/// </summary>
	public static class List
	{
		/// <summary>
		/// Helper to create a cons list without explicitly specifying the item type. 
		/// </summary>
		public static NOPList<T> Cons<T> (T first, NOPList<T> rest)
		{
			return NOPList<T>.Cons (first, rest);
		}

		/// <summary>
		/// Helper to create a cons list without explicitly specifying the item type. 
		/// </summary>
		public static NOPList<T> Cons<T> (T first)
		{
			return NOPList<T>.Cons (first, NOPList<T>.Empty);
		}

		/// <summary>
		/// Constructs a new list from an array.
		/// </summary>
		/// <param name="array">The array whose items are placed into the list.</param>
		/// <returns>A new list that contains the same items in the same order as the 
		/// array.</returns>
		public static NOPList<T> FromArray<T> (T[] array)
		{
			NOPList<T > result = NOPList<T>.Empty;
			
			for (int i = array.Length - 1; i >= 0; i--)
				result = NOPList<T>.Cons (array [i], result);
			return result;
		}

		/// <summary>
		/// Constructs a new list from a variable argument list.
		/// </summary>
		/// <param name="items">The items that are placed into the list.</param>
		/// <returns>A new list that contains the items in the order specified.</returns>
		public static NOPList<T> Create<T> (params T[] items)
		{
			return FromArray (items);
		}

		/// <summary>
		/// Constructs a new list from IEnumerable.
		/// </summary>
		/// <param name="items">The items that are placed into the list.</param>
		/// <returns>A new list that contains the items in the order specified.</returns>
		public static NOPList<T> Create<T> (IEnumerable<T> items)
		{
			return NOPList<T>.FromEnumerable (items);
		}
	}
}
