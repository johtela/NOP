namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Reflection;
	using NOP.Collections;

	/// <summary>
	/// Interface for generating random values.
	/// </summary>
	/// <typeparam name="T">The type of the arbitrary value created.</typeparam>
	public interface IArbitrary<T>
	{
		T Generate (Random rnd, int size);
		IStream<T> Shrink (T value);
	}

	/// <summary>
	/// Base class for creating instances and combinators for arbitrary values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ArbitraryBase<T> : IArbitrary<T>
	{
		public abstract T Generate (Random rnd, int size);

		public IStream<T> Shrink (T value)
		{
			return List.Cons (value);
		}
	}

	/// <summary>
	/// Concrete arbitrary for non-generic types. Takes a function to
	/// create an arbitrary value.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Arbitrary<T> : ArbitraryBase<T>
	{
		public readonly Func<Random, int, T> Generator;

		public Arbitrary (Func<Random, int, T> generator)
		{
			Generator = generator;
		}

		public override T Generate (Random rnd, int size)
		{
			return Generator (rnd, size);
		}
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
			DefaultArbitrary.Register ();
		}

		public static void Register<T> (IArbitrary<T> arbitrary)
		{
			_container.Register (arbitrary);
		}

		public static void Register (Type type)
		{
			_container.Register (type);
		}

		public static IArbitrary<T> Get<T> ()
		{
			return (IArbitrary<T>)_container.GetImplementation (typeof (T));
		}

		public static T Generate<T> (Random rnd, int size)
		{
			return Get<T> ().Generate (rnd, size);
		}

		/// <summary>
		/// Monadic return lifts a value to arbitrary.
		/// </summary>
		public static IArbitrary<T> ToArbitrary<T> (this T value)
		{
			return new Arbitrary<T> ((rnd, size) => value);
		}

		/// <summary>
		/// Monadic bind, the magical wand that allows composing arbitraries.
		/// </summary>
		public static IArbitrary<U> Bind<T, U> (this IArbitrary<T> arb, Func<T, IArbitrary<U>> func)
		{
			return new Arbitrary<U> ((rnd, size) =>
			{
				var a = arb.Generate (rnd, size);
				return func (a).Generate (rnd, size);
			});
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
		/// Where extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static IArbitrary<T> Where<T> (this IArbitrary<T> arb, Func<T, bool> predicate)
		{
			return new Arbitrary<T> ((rnd, size) =>
			{
				T result;
				do { result = arb.Generate (rnd, size); }
				while (!predicate (result));
				return result;
			});
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
