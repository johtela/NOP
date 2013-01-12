namespace NOP.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class Enumerable
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T item)
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
    }
}
