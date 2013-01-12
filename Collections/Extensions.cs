namespace NOP.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    /// <summary>
    /// Extension methods for .NET framework classes.
    /// </summary>
    public static class Extensions
	{
        #region Array extensions

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

        #endregion

        #region IEnumerable extensions

        public static IEnumerable<T> Append<T> (this IEnumerable<T> enumerable, T item)
        {
            foreach (T i in enumerable)
                yield return i;
            yield return item;
        }

        public static IEnumerable<T> Prepend<T> (this IEnumerable<T> enumerable, T item)
        {
            yield return item;
            foreach (T i in enumerable)
                yield return i;
        }

        #endregion

        #region Tuple extensions
		        
        public static void Bind<T, U> (this Tuple<T, U> tuple, Action<T, U> action)
        {
            action (tuple.Item1, tuple.Item2);
        }

        public static V Bind<T, U, V> (this Tuple<T, U> tuple, Func<T, U, V> func)
        {
            return func (tuple.Item1, tuple.Item2);
        }

        public static bool Match<T, U> (this Tuple<T, U> tuple, T first, out U second)
        {
            if (tuple.Item1.Equals (first))
            {
                second = tuple.Item2;
                return true;
            }
            else
            {
                second = default (U);
                return false;
            }
        }

        public static bool Match<T, U> (this Tuple<T, U> tuple, out T first, U second)
        {
            if (tuple.Item2.Equals (second))
            {
                first = tuple.Item1;
                return true;
            }
            else
            {
                first = default (T);
                return false;
            }
        }

        public static bool Match<T, U> (this Tuple<T, U> tuple, Func<T, bool> predicate, out U second)
        {
            if (predicate (tuple.Item1))
            {
                second = tuple.Item2;
                return true;
            }
            else
            {
                second = default (U);
                return false;
            }
        }

	    #endregion    

        #region String extensions
		       
        public static string Times (this string what, int times)
        {
            var sb = new StringBuilder ();

            for (int i = 0; i < times; i++)
                sb.Append (what);

            return sb.ToString ();
        }

	    #endregion    
    }
}