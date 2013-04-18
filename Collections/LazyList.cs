namespace NOP.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class LazyList<T> : ISequence<T>
	{
		protected static readonly LazyList<T> _empty = 
			new LazyList<T> (default (T), (LazyList<T>)null);
		private T _first;
		private LazyList<T> _rest;
		private Func<LazyList<T>> _getRest;

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
		public LazyList<T> Rest
		{ 
			get
			{
				if (this == _empty)
					throw new EmptyListException ();
				if (_rest == null)
					_rest = _getRest ();
				return _rest;
			}
		}

		ISequence<T> ISequence<T>.Rest
		{
			get { return Rest; }
		}

		private LazyList (T first, Func<LazyList<T>> getRest)
		{
			_first = first;
			_getRest = getRest;
		}

		private LazyList (T first, LazyList<T> rest)
		{
			_first = first;
			_rest = rest;
		}

		/// <summary>
		/// Create a list by appending an item at head of the list.
		/// </summary>
		public static LazyList<T> Cons (T first, Func<LazyList<T>> getRest)
		{
			return new LazyList<T> (first, getRest);
		}

		private static LazyList<T> Enumerate (IEnumerator<T> e)
		{
			return !e.MoveNext () ? _empty :
				new LazyList<T> (e.Current, Fun.Partial (Enumerate, e));
		}

		/// Construct a list from an enumerable.
		/// </summary>
		public static LazyList<T> FromEnumerable (IEnumerable<T> values)
		{
			return Enumerate (values.GetEnumerator ());
		}

		/// <summary>
		/// Return an empty list, i.e. null.
		/// </summary>
		public static LazyList<T> Empty
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
		/// Concatenate two lazy lists
		/// </summary>
		private LazyList<T> Concat (LazyList<T> other)
		{
			return IsEmpty ? other :
				Cons (First, Fun.Partial (Rest.Concat, other));
		}

		/// <summary>
		/// Collect the list of lists into another list.
		/// </summary>
		public LazyList<U> Collect<U> (Func<T, LazyList<U>> func)
		{
			return IsEmpty ? LazyList<U>.Empty :
				func (First).Concat (Rest.Collect (func));
		}

		/// <summary>
		/// Map list to another list. 
		/// </summary>
		public LazyList<U> Map<U> (Func<T, U> func)
		{
			return IsEmpty ? LazyList<U>.Empty :
				new LazyList<U> (func (First), Fun.Partial (Rest.Map, func));
		}

		#region Operator overloads

		public static LazyList<T> operator | (T first, LazyList<T> rest)
		{
			return new LazyList<T> (first, rest);
		}

		#endregion
	}
}