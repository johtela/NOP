using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOP
{
	public abstract class NOPFunc<TArg, TRes>
	{
		public abstract TRes Invoke (TArg arg);
	}

	public abstract class NOPFunc<TArg1, TArg2, TRes> : 
        NOPFunc<TArg1, NOPFunc<TArg2, TRes>>
	{
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

		public abstract TRes Invoke (TArg1 arg1, TArg2 arg2);

		public override NOPFunc<TArg2, TRes> Invoke (TArg1 arg1)
		{
			return new Curried (this, arg1);
		}
	}

	public abstract class NOPFunc<TArg1, TArg2, TArg3, TRes> : 
        NOPFunc<TArg1, NOPFunc<TArg2, NOPFunc<TArg3, TRes>>>
	{
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

		public abstract TRes Invoke (TArg1 arg1, TArg2 arg2, TArg3 arg3);

		public override NOPFunc<TArg2, NOPFunc<TArg3, TRes>> Invoke (TArg1 arg1)
		{
			return new Curried (this, arg1);
		}
	}

	public abstract class NOPFunc<TArg1, TArg2, TArg3, TArg4, TRes> : 
        NOPFunc<TArg1, NOPFunc<TArg2, NOPFunc<TArg3, NOPFunc<TArg4, TRes>>>>
	{
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

		public abstract TRes Invoke (TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

		public override NOPFunc<TArg2, NOPFunc<TArg3, NOPFunc<TArg4, TRes>>> Invoke (TArg1 arg1)
		{
			return new Curried (this, arg1);
		}
	}
}
