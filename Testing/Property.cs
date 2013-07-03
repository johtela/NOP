namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public delegate void Property (TestState state);

	public class TestState
	{
		public readonly Random Random;
		public readonly int Size;
		public readonly string Label;

		public TestState () : this (new Random (), 10, null) {}

		public TestState (Random random, int size, string label)
		{
			Random = random;
			Size = size;
			Label = label;
		}
	}

	public static class Prop
	{
		private static void Failed<T> (TestState state, T input)
		{
			throw new TestFailed (string.Format ("Property '{0}' failed for input {1}", 
				state.Label, input));
		}

		private static void Test<T> (IArbitrary<T> arb, TestState state, Func<T, bool> func)
		{
			var input = arb.Generate (state.Random, state.Size); 
			if (!func (input))
				Failed (state, input);
		}

		public static Property Lift<T> (Func<T, bool> func)
		{
			return s => Test (Arbitrary.Get<T> (), s, func);
		}

		public static Property Lift<T, U> (Func<T, U, bool> func)
		{
			return s => Test (Arbitrary.Get<T> ().Plus (Arbitrary.Get<U> ()), s, Fun.Tuplize(func));
		}

		public static Property Lift<T, U, V> (Func<T, U, V, bool> func)
		{
			return s => Test (Arbitrary.Get<T> ().Plus (Arbitrary.Get<U> (), Arbitrary.Get<V> ()), 
				s, Fun.Tuplize (func));
		}

		public static Property Label (this Property property, string label)
		{
			return s => property (new TestState (s.Random, s.Size, label));
		}

		public static void Check (this Property prop, int tries)
		{
			var state = new TestState ();

			for (int i = 0; i < tries; i++)
				prop (state);
		}

		public static void Check (this Property prop)
		{
			Check (prop, 100);
		}
	}
}
