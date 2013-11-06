namespace NOP
{
	using System;

	public static class Prelude
	{
		public static void Set<T> (ref T variable, T value)
		{
			variable = value;
		}

		public static bool Eq<T> (T value1, T value2)
		{
			return value1.Equals (value2);
		}
	}
}
