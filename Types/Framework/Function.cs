namespace NOP.Framework
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
		private MethodBase _methodBase;
			
		public Function (Func call) : base (null)
		{
			if (call == null)
				throw new ArgumentNullException ("call");
			_call = call;
		}
			
		public Function (MethodBase mb) : base (mb)
		{
			if (!(mb is ConstructorInfo || mb.IsStatic))
				throw new ArgumentException ("Method wrapped need to be static");
			_methodBase = mb;
		}
		
		public override string ToString ()
		{
			if (_methodBase is MethodInfo) 
				return base.ToString ();
			var sig = _methodBase.ToString ();
			return _methodBase.DeclaringType.Name + sig.Substring (sig.IndexOf ('('));
		}

		public Func Call
		{
			get
			{
				if (_call == null)
				{
					_call = _methodBase is ConstructorInfo ?
						(_methodBase as ConstructorInfo).AsFunction () :
						(_methodBase as MethodInfo).AsFunction ();
				}
				return _call;
			}
		}
		
		public MethodBase MethodBase
		{
			get { return _methodBase; }
		}
	}
}