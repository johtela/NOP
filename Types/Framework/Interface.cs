using System;
using System.Linq;
using NOP.Collections;

namespace NOP.Framework
{
	/// <summary>
	/// Interface contains just abstract members, and no static definitions.
	/// </summary>
	public class Interface : TypeDefinition
	{
		public Interface (Namespace parent, Type type) :
			base(parent, type)
		{
			_definitions = Map<string, Definition>.FromPairs (
				Methods ().Concat (Properties ()));
		}
	}
}

