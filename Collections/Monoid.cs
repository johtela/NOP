namespace NOP.Collections
{
	using System;

	public interface IMonoid<T> where T : new () 
	{
		T Plus (T other);
	}

	public interface IMeasurable<V> where V: IMonoid<V>, new ()
	{
		V Measure ();
	}

	public struct TreeSize : IMonoid<TreeSize>
	{
		private int _value;

		public TreeSize (int value)
		{
			_value = value;
		}

		public static implicit operator int (TreeSize ts)
		{
			return ts._value;
		}

		public TreeSize Plus (TreeSize other)
		{
			return new TreeSize (_value + other._value);
		}
	}

	public struct Elem<T> : IMeasurable<TreeSize>
	{
		public readonly T Value;

		public Elem (T value)
		{
			Value = value;
		}

		public static implicit operator T (Elem<T> e)
		{
			return e.Value;
		}

		public TreeSize Measure ()
		{
			return new TreeSize (1);
		}
	}

	public static class Elem
	{
		public static Elem<T> Create<T> (T value)
		{
			return new Elem<T> (value);
		}
	}
}
