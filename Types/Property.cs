using System;
using System.Reflection;
using NOP.Collections;
using ExprList = NOP.Collections.List;

namespace NOP
{
	/// <summary>
	/// Property of a class.
	/// </summary>
	public sealed class Property : Member
	{
		private Func<object, object> _get;
		private Action<object, object> _set;
		private PropertyInfo _propertyInfo;
		
		public Property (Func<object, object> getter, Action<object, object> setter)
		{
			if (getter == null)
				throw new ArgumentNullException ("getter");
			_get = getter;
			_set = setter;
		}
			
		public Property (PropertyInfo pi)
		{
			if (pi == null)
				throw new ArgumentNullException ("pi");
			_propertyInfo = pi;
		}
			
		public Func<object,object> Get
		{
			get
			{
				if (_get == null)
				{
					_get = _propertyInfo.AsPropertyGetter ();
				}
				return _get;
			}
		}

		public Action<object,object> Set
		{
			get
			{
				if (_set == null)
				{
					if (!_propertyInfo.CanWrite)
						throw new NOPException ("Property is read-only");
					_set = _propertyInfo.AsPropertySetter ();
				}
				return _set;
			}
		}
	}
}