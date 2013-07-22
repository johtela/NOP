﻿namespace NOP.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class LazyList<T> : ISequence<T>
	{
		protected static readonly LazyList<T> _empty = new LazyList<T> (default (T), (LazyList<T>)null);
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

		object IStream.First
		{
			get { return First; }
		}

		IStream IStream.Rest
		{
			get { return Rest; }
		}

		IStream<T> IStream<T>.Rest
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

		public static LazyList<T> Cons (T first, LazyList<T> rest)
		{
			return new LazyList<T> (first, rest);
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

		/// Construct a list from an enumerable.
		/// </summary>
		public static LazyList<T> FromStream (IStream<T> seq)
		{
			return seq is LazyList<T> ? seq as LazyList<T> :
				seq.IsEmpty ? Empty :
				new LazyList<T> (seq.First, () => FromStream (seq.Rest));
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
		/// Append an item at the end of the list.
		/// </summary>
		public LazyList<T> Append (T item)
		{
			return Concat (Cons (item, Empty));
		}

		/// <summary>
		/// Concatenate two lazy lists
		/// </summary>
		public LazyList<T> Concat (LazyList<T> other)
		{
			return IsEmpty ? other :
				Cons (First, Fun.Partial (Rest.Concat, other));
		}

		/// <summary>
		/// Collect the list of lists into another list.
		/// </summary>
		public LazyList<U> Collect<U> (Func<T, LazyList<U>> func)
		{
			if (IsEmpty) 
				return LazyList<U>.Empty;
			var lst = func (First);
			return lst.IsEmpty ? 
				Rest.Collect (func) :
				new LazyList<U> (lst.First, () => lst.Rest.Concat (Rest.Collect (func)));
		}

		ISequence<U> ISequence<T>.Collect<U> (Func<T, ISequence<U>> func)
		{
			return Collect (e => LazyList<U>.FromStream (func (e)));
		}

		public LazyList<T> Reverse ()
		{
			var result = Empty;
			for (var list = this; !list.IsEmpty; list = list.Rest)
				result = new LazyList<T> (list.First, result);
			return result;
		}

		public LazyList<Tuple<T, U>> ZipWith<U> (LazyList<U> other)
		{
			return IsEmpty || other.IsEmpty ? LazyList<Tuple<T, U>>.Empty :
				new LazyList<Tuple<T, U>> (Tuple.Create (First, other.First), 
					() => Rest.ZipWith (other.Rest));
		}

		#region ISequence<T> implementation

		/// <summary>
		/// Map list to another list. 
		/// </summary>
		public LazyList<U> Map<U> (Func<T, U> map)
		{
			return IsEmpty ? LazyList<U>.Empty :
				new LazyList<U> (map (First), Fun.Partial (Rest.Map, map));
		}

		ISequence<U> ISequence<T>.Map<U> (Func<T, U> map)
		{
			return Map (map);
		}

		public LazyList<T> Filter (Func<T, bool> predicate)
		{
			var list = this;
			while (!list.IsEmpty && !predicate (list.First))
				list = list.Rest;
			return list.IsEmpty ? Empty :
				new LazyList<T> (list.First, Fun.Partial (list.Rest.Filter, predicate));
		}

		ISequence<T> ISequence<T>.Filter (Func<T, bool> predicate)
		{
			return Filter (predicate);
		}

		#endregion

		#region IReducible<T> implementation
		
		public U ReduceLeft<U> (U acc, Func<U, T, U> func)
		{
			for (var list = this; !list.IsEmpty; list = list.Rest)
				acc = func (acc, list.First);
			return acc;
		}

		public U ReduceRight<U> (Func<T, U, U> func, U acc)
		{
			for (var list = Reverse (); !list.IsEmpty; list = list.Rest)
				acc = func (list.First, acc);
			return acc;
		}

		#endregion		

		#region Operator overloads

		public static LazyList<T> operator | (T first, LazyList<T> rest)
		{
			return new LazyList<T> (first, rest);
		}

		public static LazyList<T> operator + (LazyList<T> list, T item)
		{
			return list.Append (item);
		}

		public static LazyList<T> operator + (LazyList<T> list, LazyList<T> other)
		{
			return list.Concat (other);
		}

		#endregion

		#region Overridden methods from Object

		public override bool Equals (object obj)
		{
			var otherList = obj as LazyList<T>;
			return (otherList != null) && this.IsEqualTo (otherList);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="NOP.Collections.List`1"/> object.
		/// </summary>
		public override int GetHashCode ()
		{
			return ReduceLeft (0, (h, e) => h ^ e.GetHashCode ());
		}

		/// <summary>
		/// Returns a string representing the list.
		/// </summary>
		public override string ToString ()
		{
			return this.ToString ("[", "]", ", ");
		}

		#endregion		
	}

	public static class LazyList
	{
		/// <summary>
		/// Helper to create a cons list without explicitly specifying the item type. 
		/// </summary>
		public static LazyList<T> Cons<T> (T first, LazyList<T> rest)
		{
			return LazyList<T>.Cons (first, rest);
		}

		/// <summary>
		/// Helper to create a cons list without explicitly specifying the item type. 
		/// </summary>
		public static LazyList<T> Cons<T> (T first)
		{
			return LazyList<T>.Cons (first, LazyList<T>.Empty);
		}

		/// <summary>
		/// Constructs a new list from an array.
		/// </summary>
		public static LazyList<T> FromArray<T> (T[] array)
		{
			return array.ReduceRight (LazyList<T>.Cons, LazyList<T>.Empty);
		}

		/// <summary>
		/// Constructs a new list from a variable argument list.
		/// </summary>
		public static LazyList<T> Create<T> (params T[] items)
		{
			return FromArray (items);
		}

		/// <summary>
		/// Constructs a new list from IEnumerable.
		/// </summary>
		public static LazyList<T> FromEnumerable<T> (IEnumerable<T> items)
		{
			return LazyList<T>.FromEnumerable (items);
		}
	}
}