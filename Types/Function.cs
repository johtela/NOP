namespace NOP
{
	using System;
	using System.Reflection;
	using NOP.Collections;

	/// <summary>
	/// A function that is defined in context of the module.
	/// </summary>
	public class Function : Definition
	{
		private Func _call;
		private MethodInfo _methodInfo;
			
		public Function (Func call) : base (null)
		{
			if (call == null)
				throw new ArgumentNullException ("call");
			_call = call;
		}
			
		public Function (MethodInfo mi) : base (mi)
		{
			if (!(mi as MethodInfo).IsStatic)
				throw new ArgumentException ("Method wrapped need to be static");
			_methodInfo = mi;
		}

		public Func Call
		{
			get
			{
				if (_call == null)
				{
					_call = _methodInfo.AsFunction ();
				}
				return _call;
			}
		}
		
		public MethodInfo MethodInfo
		{
			get { return _methodInfo; }
		}
	}
}