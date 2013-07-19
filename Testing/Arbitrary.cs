namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Reflection;

	public interface IArbitrary
	{
		object Generate (Random rnd, int size);
	}

	/// <summary>
	/// The interface for generating random values.
	/// </summary>
	/// <typeparam name="T">The type of the arbitrary value created.</typeparam>
	public interface IArbitrary<out T> : IArbitrary
	{
		new T Generate (Random rnd, int size);
	}

	public abstract class ArbitraryBase<T> : IArbitrary<T>
	{
		public abstract T Generate (Random rnd, int size);

		object IArbitrary.Generate (Random rnd, int size)
		{
			return Generate (rnd, size);
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
			_container.Register (Assembly.GetAssembly (typeof (Arbitrary)));
		}

		public static void Register<T> (IArbitrary<T> arbitrary)
		{
			_container.Register (arbitrary);
		}

		public static IArbitrary<T> Get<T> ()
		{
			return (IArbitrary<T>)_container.GetImplementation (typeof (T));
		}

		public static IArbitrary Get (Type type)
		{
			return (IArbitrary)_container.GetImplementation (type);
		}

		public static T Generate<T> (Random rnd, int size)
		{
			return Get<T> ().Generate (rnd, size);
		}

		/// <summary>
		/// Monadic return implementation.
		/// </summary>
		private class MReturn<T> : ArbitraryBase<T>
		{
			public readonly T Value;

			public MReturn (T value)
			{
				Value = value;
			}

			public override T Generate (Random rnd, int size)
			{
				return Value;
			}
		}

		/// <summary>
		/// Monadic bind implementation.
		/// </summary>
		private class MBind<T, U> : ArbitraryBase<U>
		{
			public readonly IArbitrary<T> Arbitrary;
			public readonly Func<T, IArbitrary<U>> Function;

			public MBind (IArbitrary<T> arb, Func<T, IArbitrary<U>> func)
			{
				Arbitrary = arb;
				Function = func;
			}

			public override U Generate (Random rnd, int size)
			{
				var a = Arbitrary.Generate (rnd, size);
				return Function (a).Generate (rnd, size);
			}
		}

		private class MWhere<T> : ArbitraryBase<T>
		{
			public readonly IArbitrary<T> Arbitrary;
			public readonly Func<T, bool> Predicate;

			public MWhere (IArbitrary<T> arbitrary, Func<T, bool> predicate)
			{
				Arbitrary = arbitrary;
				Predicate = predicate;
			}

			public override T Generate (Random rnd, int size)
			{
				T result;
				do { result = Arbitrary.Generate (rnd, size); } 
				while (!Predicate (result));
				return result;
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

		public static IArbitrary<T> Where<T> (this IArbitrary<T> arb, Func<T, bool> predicate)
		{
			return new MWhere<T> (arb, predicate);
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

		private class MCombine : ArbitraryBase<object[]>
		{
			public readonly IEnumerable<IArbitrary> _arbitraries;

			public MCombine (IEnumerable<IArbitrary> arbs)
			{
				_arbitraries = arbs;
			}

			public override object[] Generate (Random rnd, int size)
			{
				return _arbitraries.Select (arb => arb.Generate (rnd, size)).ToArray ();
			}
		}

		public static IArbitrary<object[]> Combine (this IEnumerable<IArbitrary> arbs)
		{
			return new MCombine (arbs);
		}
	}
}
