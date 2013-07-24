namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Collections;

	internal static class DefaultArbitrary
	{
		internal static void Register ()
		{
			var charCandidates = CharCandidates ().ToArray ();

			Arbitrary.Register (new Arbitrary<char> ((rnd, size) =>
				charCandidates[rnd.Next (charCandidates.Length)],
				ShrinkChar));

			Arbitrary.Register (new Arbitrary<int> ((rnd, size) => rnd.Next (size),
				x => ShrinkInteger (x).Distinct ()));

			Arbitrary.Register (new Arbitrary<long> ((rnd, size) => rnd.Next (size), 
				x => ShrinkInteger ((int)x).Distinct ().Cast<long> ()));

			Arbitrary.Register (new Arbitrary<float> ((rnd, size) =>
				(float)rnd.NextDouble () * size));

			Arbitrary.Register (new Arbitrary<double> ((rnd, size) =>
				rnd.NextDouble () * size));

			Arbitrary.Register (typeof (Enumerable<>));
			Arbitrary.Register (typeof (Array<>));
			Arbitrary.Register (new Arbitrary<string> ((rnd, size) =>
				new string (Arbitrary.Generate<char[]> (rnd, size)),
				x => ShrinkEnumerable (x).Select (cs => new string (cs.ToArray ()))));

			Arbitrary.Register (typeof (AStrictList<>));
			Arbitrary.Register (typeof (ALazyList<>));
			Arbitrary.Register (typeof (ASequence<>));
		}

		private static IEnumerable<char> CharCandidates ()
		{
			for (char c = ' '; c <= '~'; c++)
				yield return c;
			yield return '\t';
			yield return '\n';
		}

		private static IEnumerable<char> ShrinkChar (char c)
		{
			var candidates = new char[] 
				{ 'a', 'b', 'c', 'A', 'B', 'C', '1', '2', '3', char.ToLower (c), ' ', '\n' };

			return candidates.Where (x => x.SimplerThan (c)).Distinct ();
		}

		private static bool SimplerThan (this char x, char y)
		{
			Func<Func<char, bool>, bool> simpler = fun => fun (x) && !fun (y);

			return simpler (char.IsLower) || simpler (char.IsUpper) || simpler (char.IsDigit) ||
				simpler (c => c == ' ') || simpler (char.IsWhiteSpace) || x < y;
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
			return RemoveUntil (e).Collect (Fun.Identity).Concat (ShrinkOne (e)).Prepend (new T[0]);
		}

		private static IEnumerable<IEnumerable<IEnumerable<T>>> RemoveUntil<T> (IEnumerable<T> e)
		{
			var len = e.Count ();
			for (var k = len - 1; k > 0; k = k / 2)
				yield return RemoveK (e, k, len);
		}

		private static IEnumerable<IEnumerable<T>> RemoveK<T> (IEnumerable<T> e, int k, int len)
		{
			if (k > len) return new IEnumerable<T>[0];
			var xs1 = e.Take (k);
			var xs2 = e.Skip (k);
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
				return ShrinkEnumerable (value.ToEnumerable ()).Select (List.FromEnumerable);
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
				return ShrinkEnumerable (value.ToEnumerable ()).Select (LazyList.FromEnumerable);
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
				return ShrinkEnumerable (value.ToEnumerable ()).Select (Sequence.FromEnumerable);
			}
		}
	}
}
