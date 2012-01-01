using System;
using System.Reflection;

namespace NOP
{
	public class Constructor : Member
	{
		private Func _create;
		private ConstructorInfo _constructorInfo;
		
		public Constructor (Func create)
		{
			_create = create;
		}
		
		public Constructor (ConstructorInfo ci)
		{
			if (ci == null)
				throw new ArgumentNullException ("ci");
			_constructorInfo = ci;
		}

		public Func Create
		{
			get
			{
				if (_create == null)
				{
					_create = _constructorInfo.AsFunction ();
				}
				return _create;
			}
		}		
	}
}

