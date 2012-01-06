using System;
using System.Linq;
using System.Reflection;
using NOP.Collections;
using ExprList = NOP.Collections.List;

namespace NOP
{
	/// <summary>
	/// Classes are by reference types that can contain all kind of definitions.
	/// </summary>
	public class Class : TypeDefinition
	{
		public Class (Namespace parent, Type type) :
			base(parent, type)
		{
			_definitions = Map<string, Definition>.FromPairs (Functions ()
				.Concat (Values ())
				.Concat (Variables ())
				.Concat (Constructors ())
				.Concat (Methods ())
				.Concat (Properties ()));
			AddNestedTypes ();
		}
	}
}