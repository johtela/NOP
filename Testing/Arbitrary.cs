namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Reflection;

	public interface IArbitrary<out T>
	{
		T Generate (Random rnd, int size);
	}

	public static class Arbitrary
	{
		private static Container _container;

		static Arbitrary ()
		{
			_container = new Container (typeof (IArbitrary<>));
			_container.Register (Assembly.GetAssembly (typeof (Arbitrary)));
		}

		public static IArbitrary<T> Get<T> ()
		{
			return _container.GetImplementation<IArbitrary<T>> (typeof (T));
		}

		public static T Generate<T> (Random rnd, int size)
		{
			return Get<T> ().Generate (rnd, size);
		}

		private class Integer : IArbitrary<int>
		{
			public int Generate (Random rnd, int size)
			{
				return rnd.Next (size);
			}
		}

		private class Char : IArbitrary<char>
		{
			public char Generate (Random rnd, int size)
			{
				return Convert.ToChar (rnd.Next (Convert.ToInt32 (' '), Convert.ToInt32 ('~')));
			}
		}

		private class Enumerable<T> : IArbitrary<IEnumerable<T>>
		{
			public IEnumerable<T> Generate (Random rnd, int size)
			{
				var len = rnd.Next (size);
				for (int i = 0; i < len; i++)
					yield return Generate<T> (rnd, size);
			}
		}

		private class Array<T> : IArbitrary<T[]>
		{
			public T[] Generate (Random rnd, int size)
			{
				return Generate<IEnumerable<T>> (rnd, size).ToArray ();
			}
		}

		private class String : IArbitrary<string>
		{
			public string Generate (Random rnd, int size)
			{
				return new string (Generate<char[]> (rnd, size));
			}
		}
	}
}
