using System;
using System.Reflection;
using NOP.Collections;
using ExprList = NOP.Collections.List;

namespace NOP
{
	/// <summary>
	/// A mutable variable.
	/// </summary>
	public class Variable : Value
	{
		private Action<object> _set;
		
		public Variable (Func<object> getter, Action<object> setter) : base (getter)
		{
			if (_set == null)
				throw new ArgumentNullException ("setter");
			_set = setter;
		}
			
		public Variable (MemberInfo mi) : base(mi)
		{
		}
			
		public Action<object> Set
		{
			get
			{
				if (_set == null)
				{
					if (_memberInfo is FieldInfo)
						_set = (_memberInfo as FieldInfo).AsVariableSetter ();
					else
					if (_memberInfo is PropertyInfo)
						_set = (_memberInfo as PropertyInfo).AsVariableSetter ();
				}
				return _set;
			}
		}
	}
}