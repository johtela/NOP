namespace NOP.Collections
{
    using System;
    using System.Collections.Generic;

    public struct SubArray<T> : IEnumerable<T>
	{
		private readonly T[] _array;
		private readonly int _first, _count;
		
		public SubArray (T[] array)
		{
			_array = array;
			_first = 0;
			_count = array.Length;
		}
		
		public SubArray (T[] array, int count)
		{
			if (count < 0 || count > array.Length)
				throw new ArgumentException ("Count is out of array index range", "count");
			_array = array;
			_first = 0;
			_count = count;
		}
		
		public SubArray (T[] array, int first, int count)
		{
			if (first < 0 || first >= array.Length)
				throw new ArgumentException ("First is out of array index range", "first");
			if (count < 0 || (first + count) > array.Length)
				throw new ArgumentException ("Count is out of array index range", "count");
			_array = array;
			_first = first;
			_count = count;
		}
		
		public T[] CopyToArray ()
		{
			var result = new T[_count];
			Array.Copy (_array, _first, result, 0, _count);
			return result;
		}

		public T this [int index]
		{
			get
			{
				if (index < 0 || index >= _count)
					throw new IndexOutOfRangeException ();
				return _array [index - _first]; 
			}
		}
		
		public int Length
		{
			get { return _count; }
		}
		
		#region IEnumerable[T] implementation
		
		public IEnumerator<T> GetEnumerator ()
		{
			for (int i = _first; i < (_first + _count); i++)
			{
				yield return _array[i];
			}
		}
		
		#endregion

		#region IEnumerable implementation
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
		
		#endregion
	}
}