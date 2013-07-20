namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Collections;

	internal class DefaultArbitrary
	{
		internal static void Register ()
		{
			Arbitrary.Register (new Arbitrary<char> ((rnd, size) =>
				Convert.ToChar (rnd.Next (Convert.ToInt32 (' '), Convert.ToInt32 ('~')))));
			Arbitrary.Register (new Arbitrary<int> ((rnd, size) => rnd.Next (size)));
			Arbitrary.Register (new Arbitrary<long> ((rnd, size) =>
				rnd.Next (size) << 32 + rnd.Next (size)));
			Arbitrary.Register (new Arbitrary<float> ((rnd, size) =>
				(float)rnd.NextDouble () * size));
			Arbitrary.Register (new Arbitrary<double> ((rnd, size) =>
				rnd.NextDouble () * size));
			Arbitrary.Register (typeof (Enumerable<>));
			Arbitrary.Register (typeof (Array<>));
			Arbitrary.Register (new Arbitrary<string> ((rnd, size) =>
				new string (Arbitrary.Generate<char[]> (rnd, size))));
			Arbitrary.Register (typeof (AStrictList<>));
			Arbitrary.Register (typeof (ALazyList<>));
			Arbitrary.Register (typeof (ASequence<>));
		}

		private class Enumerable<T> : ArbitraryBase<IEnumerable<T>>
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
