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
		
		public Function GetFunction (string signature)
		{
			return (Function)Definitions [signature];
		}
	
		public Value GetValue (string name)
		{
			return (Value)Definitions [name];
		}

		public Variable GetVariable (string name)
		{
			return (Variable)Definitions [name];
		}
		
		public Constructor GetConstructor (string signature)
		{
			return (Constructor)Definitions [signature];
		}

		public Method GetMethod (string signature)
		{
			return (Method)Definitions [signature];
		}

		public Property GetProperty (string name)
		{
			return (Property)Definitions [name];
		}
	}
}