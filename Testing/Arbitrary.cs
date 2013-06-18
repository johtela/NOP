namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Reflection;

	/// <summary>
	/// The interface for generating random values.
	/// </summary>
	/// <typeparam name="T">The type of the arbitrary value created.</typeparam>
	public interface IArbitrary<out T>
	{
		T Generate (Random rnd, int size);
	}

	/// <summary>
	/// The basic infrastructure and extension methods for managing
	/// and composing IArbitrary[T] interfaces.
	/// </summary>
	public static class Arbitrary
	{
		private static Container _container;

		static Arbitrary ()
		{
			_container = new Container (typeof (IArbitrary<>));
			_container.Register (Assembly.GetAssembly (typeof (Arbitrary)));
		}

		public static void Register<T> (IArbitrary<T> arbitrary)
		{
			_container.Register (arbitrary);
		}

		public static IArbitrary<T> Get<T> ()
		{
			return _container.GetImplementation<IArbitrary<T>> (typeof (T));
		}

		public static T Generate<T> (Random rnd, int size)
		{
			return Get<T> ().Generate (rnd, size);
		}

		/// <summary>
		/// Monadic return implementation.
		/// </summary>
		private class MReturn<T> : IArbitrary<T>
		{
			public readonly T Value;

			public MReturn (T value)
			{
				Value = value;
			}

			public T Generate (Random rnd, int size)
			{
				return Value;
			}
		}

		/// <summary>
		/// Monadic bind implementation.
		/// </summary>
		private class MBind<T, U> : IArbitrary<U>
		{
			public readonly IArbitrary<T> Arbitrary;
			public readonly Func<T, IArbitrary<U>> Function;

			public MBind (IArbitrary<T> arb, Func<T, IArbitrary<U>> func)
			{
				Arbitrary = arb;
				Function = func;
			}

			public U Generate (Random rnd, int size)
			{
				var a = Arbitrary.Generate (rnd, size);
				return Function (a).Generate (rnd, size);
			}
		}

		/// <summary>
		/// Monadic return lifts a value to arbitrary.
		/// </summary>
		public static IArbitrary<T> ToArbitrary<T> (this T value)
		{
			return new MReturn<T> (value);
		}

		/// <summary>
		/// Monadic bind, the magical wand that allows composing arbitraries.
		/// </summary>
		public static IArbitrary<U> Bind<T, U> (this IArbitrary<T> arb, Func<T, IArbitrary<U>> func)
		{
			return new MBind<T, U> (arb, func);
		}

		/// <summary>
		/// Select extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static IArbitrary<U> Select<T, U> (this IArbitrary<T> arb, Func<T, U> select)
		{
			return arb.Bind (a => select (a).ToArbitrary ());
		}

		/// <summary>
		/// SelectMany extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static IArbitrary<V> SelectMany<T, U, V> (this IArbitrary<T> arb,
			Func<T, IArbitrary<U>> project, Func<T, U, V> select)
		{
			return arb.Bind (a => project (a).Bind (b => select (a, b).ToArbitrary ()));
		}

		/// <summary>
		/// Combine two arbitrary values into a tuple.
		/// </summary>
		public static IArbitrary<Tuple<T, U>> Plus<T, U> (this IArbitrary<T> arb1, IArbitrary<U> arb2)
		{
			return from a in arb1
				   from b in arb2
				   select Tuple.Create (a, b);
		}

		/// <summary>
		/// Combine three arbitrary values into a tuple.
		/// </summary>
		public static IArbitrary<Tuple<T, U, V>> Plus<T, U, V> (this IArbitrary<T> arb1, IArbitrary<U> arb2,
			IArbitrary<V> arb3)
		{
			return from a in arb1
				   from b in arb2
				   from c in arb3
				   select Tuple.Create (a, b, c);
		}
	}
}
