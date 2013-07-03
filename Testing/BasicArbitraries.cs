namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	class BasicArbitraries
	{
		private class AChar : IArbitrary<Char>
		{
			public Char Generate (Random rnd, int size)
			{
				return Convert.ToChar (rnd.Next (Convert.ToInt32 (' '), Convert.ToInt32 ('~')));
			}
		}

		private class AInt32 : IArbitrary<Int32>
		{
			public Int32 Generate (Random rnd, int size)
			{
				return rnd.Next (size);
			}
		}

		private class AInt64 : IArbitrary<Int64>
		{
			public Int64 Generate (Random rnd, int size)
			{
				return rnd.Next (size) << 32 + rnd.Next (size);
			}
		}

		private class AEnumerable<T> : IArbitrary<IEnumerable<T>>
		{
			public IEnumerable<T> Generate (Random rnd, int size)
			{
				var len = rnd.Next (size);
				for (int i = 0; i < len; i++)
					yield return Arbitrary.Generate<T> (rnd, size);
			}
		}

		private class Array<T> : IArbitrary<T[]>
		{
			public T[] Generate (Random rnd, int size)
			{
				return Arbitrary.Generate<IEnumerable<T>> (rnd, size).ToArray ();
			}
		}

		private class String : IArbitrary<string>
		{
			public string Generate (Random rnd, int size)
			{
				return new string (Arbitrary.Generate<char[]> (rnd, size));
			}
		}

		private class AList<T> : IArbitrary<StrictList<T>>
		{
			public StrictList<T> Generate (Random rnd, int size)
			{
				return List.FromEnumerable (Arbitrary.Generate<IEnumerable<T>> (rnd, size));
			}
		}
	}
}	
