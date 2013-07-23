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
			Arbitrary.Register (new Arbitrary<int> ((rnd, size) => rnd.Next (size),
			                    ShrinkInteger));
			Arbitrary.Register (new Arbitrary<long> ((rnd, size) => rnd.Next (size)));
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

		private static IEnumerable<int> ShrinkInteger (int x)
		{
			if (x < 0) yield return -x;
			yield return 0;
			for (var i = x / 2; Math.Abs (x - i) < Math.Abs (x); i = i / 2)
				yield return x - i;
		}

		private static IEnumerable<IEnumerable<T>> ShrinkEnumerable<T> (IEnumerable<T> e)
		{
			return RemoveUntil (e).Collect (x => x).Concat (ShrinkOne (e));
		}

		private static IEnumerable<IEnumerable<IEnumerable<T>>> RemoveUntil<T> (IEnumerable<T> e)
		{
			var len = e.Count ();
			for (var k = len; k > 0; k = k / 2)
			{
				yield return RemoveK (e, k, len);
			}
		}

		private static IEnumerable<IEnumerable<T>> RemoveK<T> (IEnumerable<T> e, int k, int len)
		{
			if (k > len) return new IEnumerable<T>[0];
			var xs1 = e.Take (k);
			var xs2 = e.Skip (k);
			if (xs2.IsEmpty ()) return new IEnumerable<T>[] { new T[0] };
			return (from r in RemoveK (xs2, k, len - k)
			        select xs1.Concat (r)).Append (xs2);
		}

		private static IEnumerable<IEnumerable<T>> ShrinkOne<T> (IEnumerable<T> e)
		{
			if (e.IsEmpty ()) return new IEnumerable<T>[0];
			var first = e.First ();
			var rest = e.Skip (1);
			return (from x in Arbitrary.Get<T> ().Shrink (first)
 					select rest.Append(x)).Concat (
					from xs in ShrinkOne (e.Skip (1))
					select xs.Append (first));
		}

		private class Enumerable<T> : ArbitraryBase<IEnumerable<T>>
		{
			public override IEnumerable<T> Generate (Random rnd, int size)
			{
				var len = rnd.Next (size);
				for (int i = 0; i < len; i++)
					yield return Arbitrary.Generate<T> (rnd, size);
			}

			public override IEnumerable<IEnumerable<T>> Shrink (IEnumerable<T> value)
			{
				return ShrinkEnumerable (value);
			}
		}

		private class Array<T> : ArbitraryBase<T[]>
		{
			public override T[] Generate (Random rnd, int size)
			{
				return Arbitrary.Generate<IEnumerable<T>> (rnd, size).ToArray ();
			}

			public override IEnumerable<T[]> Shrink (T[] value)
			{
				return ShrinkEnumerable (value).Select (i => i.ToArray ());
			}
		}

		private class AStrictList<T> : ArbitraryBase<StrictList<T>>
		{
			public override StrictList<T> Generate (Random rnd, int size)
			{
				return List.FromEnumerable (Arbitrary.Generate<IEnumerable<T>> (rnd, size));
			}

			public override IEnumerable<StrictList<T>> Shrink (StrictList<T> value)
			{
				return ShrinkEnumerable (value.ToEnumerable ()).Select (i => List.FromEnumerable(i));
			}
		}

		private class ALazyList<T> : ArbitraryBase<LazyList<T>>
		{
			public override LazyList<T> Generate (Random rnd, int size)
			{
				return LazyList.FromEnumerable (Arbitrary.Generate<IEnumerable<T>> (rnd, size));
			}

			public override IEnumerable<LazyList<T>> Shrink (LazyList<T> value)
			{
				return ShrinkEnumerable (value.ToEnumerable ()).Select (i => LazyList.FromEnumerable(i));
			}
		}

		private class ASequence<T> : ArbitraryBase<Sequence<T>>
		{
			public override Sequence<T> Generate (Random rnd, int size)
			{
				return Sequence.FromEnumerable (Arbitrary.Generate<IEnumerable<T>> (rnd, size));
			}

			public override IEnumerable<Sequence<T>> Shrink (Sequence<T> value)
			{
				return ShrinkEnumerable (value.ToEnumerable ()).Select (i => Sequence.FromEnumerable(i));
			}
		}
	}
}
