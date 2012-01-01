using System;
using System.Linq;
using System.Reflection;
using NOP.Collections;
using ExprList = NOP.Collections.List;

namespace NOP
{
	/// <summary>
	/// Module that contains definitions, i.e. static values and functions.
	/// </summary>
	public class Module
	{
		public readonly Map<string, Definition> Definitions;
		
		public Module (Map<string, Definition> definitions)
		{
			Definitions = definitions;
		}
		
		public Module (Type type)
		{
			if (!(type.IsAbstract && type.IsSealed))
				throw new ArgumentException ("Type must be static (abstract AND sealed)", "type");
			var bf = BindingFlags.Static | BindingFlags.Public;
			Definitions = Map<string, Definition>.FromPairs (
					type.GetMethods (bf).Select (
						mi => Tuple.Create<string, Definition> (mi.Name, new Function (mi)))
				.Concat (
					type.GetFields (bf).Select (
						fi => Tuple.Create<string, Definition> (fi.Name, new Variable (fi))))
				.Concat (
					type.GetProperties (bf).Select (
						pi => Tuple.Create<string, Definition> (pi.Name, pi.CanWrite ? new Variable (pi) : new Value (pi))))
				);
		}
	}
}