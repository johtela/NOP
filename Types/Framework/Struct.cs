using System;
using System.Linq;
using NOP.Collections;

namespace NOP.Framework
{
	/// <summary>
	/// Struct is similar to class, but a value type.
	/// </summary>
	public class Struct : TypeDefinition
	{
		public Struct (Namespace parent, Type type) :
			base(parent, type)
		{
			_definitions = Map<string, Definition>.FromPairs (Functions ()
				.Concat (Values ())
				.Concat (Variables ())
				.Concat (Methods ())
				.Concat (Properties ())
			);
			AddNestedTypes ();
		}
	}
}

