using System;
using System.Reflection;
using NOP.Collections;
using ExprList = NOP.Collections.List;

namespace NOP.Framework
{
	/// <summary>
	/// Method of a class.
	/// </summary>
	public class Method : Member
	{
		private Meth _call;
		private MethodInfo _methodInfo;
			
		public Method (Meth call)
			: base (null)
		{
			_call = call;
		}
			
		public Method (MethodInfo mi)
			: base (mi)
		{
			if (mi == null)
				throw new ArgumentNullException ("mi");
			_methodInfo = mi;
		}

		public Meth Call
		{
			get
			{
				if (_call == null)
				{
					_call = _methodInfo.AsMethod ();
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