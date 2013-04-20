namespace NOP
{
	using System;

	/// <summary>
	/// Extension methods for functions.
	/// </summary>
	public static class Fun
	{
		public static Func<TRes> Partial<T, TRes> (
			this Func<T, TRes> func, T arg)
		{
			return () => func (arg);
		}

		public static Func<T2, TRes> Partial<T1, T2, TRes> (
			this Func<T1, T2, TRes> func, T1 arg1)
		{
			return arg2 => func (arg1, arg2);
		}

		public static Func<TRes> Partial<T1, T2, TRes> (
			this Func<T1, T2, TRes> func, T1 arg1, T2 arg2)
		{
			return () => func (arg1, arg2);
		}

		public static Func<T2, T3, TRes> Partial<T1, T2, T3, TRes> (
			this Func<T1, T2, T3, TRes> func, T1 arg1)
		{
			return (arg2, arg3) => func (arg1, arg2, arg3);
		}

		public static Func<T3, TRes> Partial<T1, T2, T3, TRes> (
			this Func<T1, T2, T3, TRes> func, T1 arg1, T2 arg2)
		{
			return arg3 => func (arg1, arg2, arg3);
		}

		public static Func<TRes> Partial<T1, T2, T3, TRes> (
			this Func<T1, T2, T3, TRes> func, T1 arg1, T2 arg2, T3 arg3)
		{
			return () => func (arg1, arg2, arg3);
		}

		public static Func<T2, T3, T4, TRes> Partial<T1, T2, T3, T4, TRes> (
			this Func<T1, T2, T3, T4, TRes> func, T1 arg1)
		{
			return (arg2, arg3, arg4) => func (arg1, arg2, arg3, arg4);
		}

		public static Func<T3, T4, TRes> Partial<T1, T2, T3, T4, TRes> (
			this Func<T1, T2, T3, T4, TRes> func, T1 arg1, T2 arg2)
		{
			return (arg3, arg4) => func (arg1, arg2, arg3, arg4);
		}

		public static Func<T4, TRes> Partial<T1, T2, T3, T4, TRes> (
			this Func<T1, T2, T3, T4, TRes> func, T1 arg1, T2 arg2, T3 arg3)
		{
			return arg4 => func (arg1, arg2, arg3, arg4);
		}

		public static Func<TRes> Partial<T1, T2, T3, T4, TRes> (
			this Func<T1, T2, T3, T4, TRes> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return () => func (arg1, arg2, arg3, arg4);
		}

		public static T Identity<T> (T arg)
		{
			return arg;
		}
	}
}
