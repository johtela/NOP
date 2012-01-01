using System;
using System.Reflection;
using NOP.Collections;
using ExprList = NOP.Collections.List;

namespace NOP
{
	/// <summary>
	/// Immutable value that is set only once at the creation time.
	/// </summary>
	public class Value : Definition
	{
		private Func<object> _get;
		protected MemberInfo _memberInfo;
			
		public Value (Func<object> getter)
		{
			if (_get == null)
				throw new ArgumentNullException ("getter");
			_get = getter;
		}
			
		public Value (MemberInfo mi)
		{
			if (!((mi is FieldInfo) || (mi is PropertyInfo)))
				throw new ArgumentException ("Needs to be field or property info", "mi");
			_memberInfo = mi;
		}
			
		public Func<object> Get
		{
			get
			{
				if (_get == null)
				{
					if (_memberInfo is FieldInfo)
						_get = (_memberInfo as FieldInfo).AsValueGetter ();
					else
					if (_memberInfo is PropertyInfo)
						_get = (_memberInfo as PropertyInfo).AsValueGetter ();
				}
				return _get;
			}
		}
	}
}