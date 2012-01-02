using System;

namespace NOP.Collections
{
	public static class ArrayExtensions
	{
		public static T[] Segment<T> (this T[] array, int first, int length)
		{
			if (first < 0 || first >= array.Length)
				throw new ArgumentException ("First is out of array index range", "first");
			if (length < 0 || (first + length) > array.Length)
				throw new ArgumentException ("Length is out of array index range", "length");
			var result = new T[length];
			Array.Copy (array, first, result, 0, length);
			return result;
		}
	}
}