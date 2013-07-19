namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	class BasicArbitraries
	{
		private class AChar : ArbitraryBase<Char>
		{
			public override Char Generate (Random rnd, int size)
			{
				return Convert.ToChar (rnd.Next (Convert.ToInt32 (' '), Convert.ToInt32 ('~')));
			}
		}

		private class AInt32 : ArbitraryBase<Int32>
		{
			public override Int32 Generate (Random rnd, int size)
			{
				return rnd.Next (size);
			}
		}

		private class AInt64 : ArbitraryBase<Int64>
		{
			public override Int64 Generate (Random rnd, int size)
			{
				return rnd.Next (size) << 32 + rnd.Next (size);
			}
		}

		private class AFloat : ArbitraryBase<float>
		{
			public override float Generate (Random rnd, int size)
			{
				return (float)rnd.NextDouble () * size;
			}
		}

		private class ADouble : ArbitraryBase<double>
		{
			public override double Generate (Random rnd, int size)
			{
				return rnd.NextDouble () * size;
			}
		}

		private class AEnumerable<T> : ArbitraryBase<IEnumerable<T>>
		{
			public override IEnumerable<T> Generate (Random rnd, int size)
			{
				var len = rnd.Next (size);
				for (int i = 0; i < len; i++)
					yield return Arbitrary.Generate<T> (rnd, size);
			}
		}

		private class Array<T> : ArbitraryBase<T[]>
		{
			public override T[] Generate (Random rnd, int size)
			{
				return Arbitrary.Generate<IEnumerable<T>> (rnd, size).ToArray ();
			}
		}

		private class String : ArbitraryBase<string>
		{
			public override string Generate (Random rnd, int size)
			{
				return new string (Arbitrary.Generate<char[]> (rnd, size));
			}
		}

		private class AStrictList<T> : ArbitraryBase<StrictList<T>>
		{
			public override StrictList<T> Generate (Random rnd, int size)
			{
				return List.FromEnumerable (Arbitrary.Generate<IEnumerable<T>> (rnd, size));
			}
		}

		private class ALazyList<T> : ArbitraryBase<LazyList<T>>
		{
			public override LazyList<T> Generate (Random rnd, int size)
			{
				return LazyList.FromEnumerable (Arbitrary.Generate<IEnumerable<T>> (rnd, size));
			}
		}

		private class ASequence<T> : ArbitraryBase<Sequence<T>>
		{
			public override Sequence<T> Generate (Random rnd, int size)
			{
				return Sequence.FromEnumerable (Arbitrary.Generate<IEnumerable<T>> (rnd, size));
			}
		}	
	}
}	
