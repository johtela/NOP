namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public delegate Tuple<TestResult, T> Property<T> (TestState state);

	public enum TestResult { Succeeded, Discarded };

	public class TestState
	{
		public Random Random;
		public int Size;
		public string Label;
		public int SuccessfulTests;
		public int DiscardedTests;
		public SortedDictionary<string, int> Classes;

		public TestState () : this (new Random (), 10, null) {}

		public TestState (Random random, int size, string label)
		{
			Random = random;
			Size = size;
			Label = label;
			Classes = new SortedDictionary<string, int> ();
		}
	}

	public static class Prop
	{
		public static Property<T> ToProperty<T> (this T value)
		{
			return state => Tuple.Create (TestResult.Succeeded, value);
		}

		public static Property<T> Fail<T> (this T value)
		{
			return state =>
			{
				throw new TestFailed (string.Format ("Property '{0}' failed for input {1}",
							state.Label, value));
			};
		}

		public static Property<T> Discard<T> (this T value)
		{
			return state => Tuple.Create (TestResult.Discarded, value);
		}

		public static Property<T> ForAll<T> (this IArbitrary<T> arbitrary)
		{
			return state => Tuple.Create (TestResult.Succeeded, 
				arbitrary.Generate (state.Random, state.Size));
		}

		public static Property<T> Choose<T> ()
		{
			return ForAll (Arbitrary.Get<T> ());
		}

		public static Property<T> Restrict<T> (this Property<T> prop, int size)
		{
			return state =>
			{
				var oldSize = state.Size;
				state.Size = size;
				var res = prop (state);
				state.Size = oldSize;
				return res;
			};
		}

		public static Property<U> Bind<T, U> (this Property<T> prop, Func<T, Property<U>> func)
		{
			return state =>
			{
				var res = prop (state);
				if (res.Item1 == TestResult.Succeeded)
					return func (res.Item2) (state);
				return Tuple.Create (res.Item1, default(U));
			};
		}

		public static Property<U> Select<T, U> (this Property<T> prop, Func<T, U> select)
		{
			return prop.Bind (a => select (a).ToProperty ());
		}

		public static Property<V> SelectMany<T, U, V> (this Property<T> prop,
			Func<T, Property<U>> project, Func<T, U, V> select)
		{
			return prop.Bind (a => project (a).Bind (b => select (a, b).ToProperty ()));
		}

		public static Property<T> Where<T> (this Property<T> prop, Func<T, bool> predicate)
		{
			return prop.Bind (value => predicate (value) ? value.ToProperty () : value.Discard ());
		}

		public static Property<T> OrderBy<T, U>(this Property<T> prop, Func<T, U> classify)
		{
			return state => 
			{
				var res = prop (state);
				var cl = classify (res.Item2).ToString ();
				int cnt = 0;
				state.Classes.TryGetValue (cl, out cnt);
				state.Classes[cl] = cnt + 1;
				return res;
			};
		}

		public static Property<T> FailIf<T> (this Property<T> prop, Func<T, bool> predicate)
		{
			return prop.Bind (value => predicate (value) ? value.ToProperty () : value.Fail ());
		}

		public static Property<T> Label<T> (this Property<T> property, string label)
		{
			return state =>
			{
				state.Label = label;
				return property (state);
			};
		}

		public static void Check<T> (this Property<T> prop, Func<T, bool> test, int tries = 100)
		{
			var state = new TestState ();
			var testProp = prop.FailIf (test);

			while (state.SuccessfulTests + state.DiscardedTests < tries)
			{
				var result = testProp (state);
				switch (result.Item1)
				{
					case TestResult.Succeeded:
						state.SuccessfulTests++;
						break;
					case TestResult.Discarded:
						state.DiscardedTests++;
						break;
				}
			}
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine ("'{0}' passed {1} tests. Discarded: {2}", 
				state.Label, state.SuccessfulTests, state.DiscardedTests);
			if (state.Classes.Count > 0)
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine ("Test case distribution:");
				foreach (var cl in state.Classes)
					Console.WriteLine ("{0}: {1:p}", cl.Key, (double)cl.Value / tries);
			}
			Console.ResetColor ();
		}
	}
}
