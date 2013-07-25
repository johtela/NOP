namespace NOP.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public delegate T Gen<T> (Random rnd, int size);

	public static class Gen
	{
		/// <summary>
		/// Monadic return lifts a value to Gen monadn.
		/// </summary>
		public static Gen<T> ToGen<T> (this T value)
		{
			return (rnd, size) => value;
		}

		/// <summary>
		/// Monadic bind, the magical wand that allows composing Gens.
		/// </summary>
		public static Gen<U> Bind<T, U> (this Gen<T> gen, Func<T, Gen<U>> func)
		{
			return (rnd, size) =>
			{
				var a = gen (rnd, size);
				return func (a) (rnd, size);
			};
		}

		/// <summary>
		/// Select extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static Gen<U> Select<T, U> (this Gen<T> gen, Func<T, U> select)
		{
			return gen.Bind (a => select (a).ToGen ());
		}

		/// <summary>
		/// SelectMany extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static Gen<V> SelectMany<T, U, V> (this Gen<T> gen,
			Func<T, Gen<U>> project, Func<T, U, V> select)
		{
			return gen.Bind (a => project (a).Bind (b => select (a, b).ToGen ()));
		}

		/// <summary>
		/// Where extension method needed to enable Linq's syntactic sugaring.
		/// </summary>
		public static Gen<T> Where<T> (this Gen<T> gen, Func<T, bool> predicate)
		{
			return (rnd, size) =>
			{
				T result;
				do { result = gen (rnd, size); }
				while (!predicate (result));
				return result;
			};
		}

		/// <summary>
		/// Combine two Gen values into a tuple.
		/// </summary>
		public static Gen<Tuple<T, U>> Plus<T, U> (this Gen<T> gen1, Gen<U> gen2)
		{
			return from a in gen1
				   from b in gen2
				   select Tuple.Create (a, b);
		}

		/// <summary>
		/// Combine three Gen values into a tuple.
		/// </summary>
		public static Gen<Tuple<T, U, V>> Plus<T, U, V> (this Gen<T> gen1, Gen<U> gen2,
			Gen<V> gen3)
		{
			return from a in gen1
				   from b in gen2
				   from c in gen3
				   select Tuple.Create (a, b, c);
		}
	}
}

