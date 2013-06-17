namespace NOP
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using NOP.Collections;
	using System.Reflection;

	public class Container
	{
		private Type _interface;
		private Dictionary<Type, Type> _types;
		private Dictionary<Type, object> _objects;

		public Container (Type intf)
		{
			if (!(intf.IsInterface && intf.IsGenericType))
				throw new ArgumentException ("Given type must be generic interface.", "intf");
			_interface = intf;
			_types = new Dictionary<Type, Type> ();
			_objects = new Dictionary<Type, object> ();
		}

		public void Register (Assembly assembly)
		{
			assembly.GetTypes ().ForEach (Register);
		}

		public void Register (Type type)
		{
			foreach (var argType in ArgumentTypes (type))
				_types.Add (GenericDef (argType), GenericDef (type));
		}

		public T GetImplementation<T> (Type argType)
		{
			object result;

			if (!_objects.TryGetValue (argType, out result))
			{
				Type implementingType = ImplementingType (argType);
				result = Activator.CreateInstance (implementingType);
				_objects.Add (argType, result);
			}
			return (T)result;
		}

		private static Type GenericDef (Type type)
		{
			return type.IsArray ? 
				typeof (Array) :
				type.IsGenericType ? 
					type.GetGenericTypeDefinition () : 
					type;
		}

		private IEnumerable<Type> ArgumentTypes (Type type)
		{
			if (type.GetConstructors ().All (x => x.GetParameters ().Length > 0))
				return new Type[0];

			return from i in type.GetInterfaces ()
				   where i.IsGenericType && i.GetGenericTypeDefinition () == _interface
				   select i.GetGenericArguments ().First ();
		}

		private Type ImplementingType (Type argType)
		{
			Type generic = GenericDef (argType);
			Type implementor;

			if (!_types.TryGetValue (generic, out implementor))
				throw new InvalidOperationException (
					"Could not find a generator for " + generic.Name + ", please register one.");

			return generic == typeof (Array) ?
				implementor.MakeGenericType (argType.GetElementType ()) :
				implementor.IsGenericTypeDefinition ?
					implementor.MakeGenericType (argType.GetGenericArguments ()) :
					implementor;
		}
	}
}
