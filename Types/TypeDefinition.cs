using System;
using System.Linq;
using System.Reflection;
using NOP.Collections;

namespace NOP
{
	public abstract class TypeDefinition : Namespace
	{
		private Map<string, Definition> _definitions = Map<string, Definition>.Empty;
		
		protected TypeDefinition (string[] path, Namespace parent, Type type) :
			base(path, parent)
		{
			var bf = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
			
			var functions = from mi in type.GetMethods(bf)
							where mi.IsPublic
							select new Function(mi);
		}
	}
}

