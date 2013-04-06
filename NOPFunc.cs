namespace NOP
{
	/// <summary>
	/// The runtime type corresponding to a function type. 
	/// The base class for function with type: a -> b
	/// </summary>
	public abstract class NOPFunc<TArg, TRes>
	{
		/// <summary>
		/// The abstract method to be overridden by function implementations.
		/// Invokes the function with given argument. 
		/// </summary>
		public abstract TRes Invoke (TArg arg);
	}

	/// <summary>
	/// The base class for functions with type: a -> b -> c
	/// </summary>
	public abstract class NOPFunc<TArg1, TArg2, TRes> : 
		NOPFunc<TArg1, NOPFunc<TArg2, TRes>>
	{
		// Curried function fixes the first argument.
		private class Curried : NOPFunc<TArg2, TRes>
		{
			private NOPFunc<TArg1, TArg2, TRes> _func;
			private TArg1 _arg1;

			public Curried (NOPFunc<TArg1, TArg2, TRes> func, TArg1 arg1)
			{
				_func = func;
				_arg1 = arg1;
			}

			public override TRes Invoke (TArg2 arg2)
			{
				return _func.Invoke (_arg1, arg2);
			}
		}

		/// <summary>
		/// The abstract method to be overridden by functions with arity 2.
		/// </summary>
		public abstract TRes Invoke (TArg1 arg1, TArg2 arg2);

		/// <summary>
		/// Overrides the inherited Invoke method by currying the function.
		/// In other words fixes a and returns b -> c.
		/// </summary>
		public override NOPFunc<TArg2, TRes> Invoke (TArg1 arg1)
		{
			return new Curried (this, arg1);
		}
	}

	/// <summary>
	/// The base class for functions with type: a -> b -> c -> d.
	/// </summary>
	public abstract class NOPFunc<TArg1, TArg2, TArg3, TRes> : 
		NOPFunc<TArg1, NOPFunc<TArg2, NOPFunc<TArg3, TRes>>>
	{
		// Curried function fixes the first argument.
		private class Curried : NOPFunc<TArg2, TArg3, TRes>
		{
			private NOPFunc<TArg1, TArg2, TArg3, TRes> _func;
			private TArg1 _arg1;

			public Curried (NOPFunc<TArg1, TArg2, TArg3, TRes> func, TArg1 arg1)
			{
				_func = func;
				_arg1 = arg1;
			}

			public override TRes Invoke (TArg2 arg2, TArg3 arg3)
			{
				return _func.Invoke (_arg1, arg2, arg3);
			}
		}

		/// <summary>
		/// The abstract method to be overridden by functions with arity 3.
		/// </summary>
		public abstract TRes Invoke (TArg1 arg1, TArg2 arg2, TArg3 arg3);

		/// <summary>
		/// Overrides the inherited Invoke method by currying the function.
		/// In other words fixes a and returns b -> (c -> d).
		/// </summary>
		public override NOPFunc<TArg2, NOPFunc<TArg3, TRes>> Invoke (TArg1 arg1)
		{
			return new Curried (this, arg1);
		}
	}

	/// <summary>
	/// The base class for functions with type: a -> b -> c -> d -> e.
	/// </summary>
	public abstract class NOPFunc<TArg1, TArg2, TArg3, TArg4, TRes> : 
		NOPFunc<TArg1, NOPFunc<TArg2, NOPFunc<TArg3, NOPFunc<TArg4, TRes>>>>
	{
		// Curried function fixes the first argument.
		private class Curried : NOPFunc<TArg2, TArg3, TArg4, TRes>
		{
			private NOPFunc<TArg1, TArg2, TArg3, TArg4, TRes> _func;
			private TArg1 _arg1;

			public Curried (NOPFunc<TArg1, TArg2, TArg3, TArg4, TRes> func, TArg1 arg1)
			{
				_func = func;
				_arg1 = arg1;
			}

			public override TRes Invoke (TArg2 arg2, TArg3 arg3, TArg4 arg4)
			{
				return _func.Invoke (_arg1, arg2, arg3, arg4);
			}
		}

		/// <summary>
		/// The abstract method to be overridden by functions with arity 4.
		/// </summary>
		public abstract TRes Invoke (TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

		/// <summary>
		/// Overrides the inherited Invoke method by currying the function.
		/// In other words fixes a and returns b -> (c -> (d -> e)).
		/// </summary>
		public override NOPFunc<TArg2, NOPFunc<TArg3, NOPFunc<TArg4, TRes>>> Invoke (TArg1 arg1)
		{
			return new Curried (this, arg1);
		}
	}
}
