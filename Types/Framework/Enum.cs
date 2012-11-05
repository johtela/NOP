using System;
using System.Collections.Generic;
using System.Linq;
using NOP.Collections;
using NameDef = System.Tuple<string, NOP.Framework.Definition>;

namespace NOP.Framework
{
	/// <summary>
	/// Enumurations just contain integer values.
	/// </summary>
	public class Enum : TypeDefinition
	{
		public Enum (Namespace parent, Type type) :
			base(parent, type)
		{
			_definitions = Map<string, Definition>.FromPairs (GetEnumValues ());
		}
		
		private IEnumerable<NameDef> GetEnumValues ()
		{
			var names = _type.GetEnumNames ();
			var values = _type.GetEnumValues ();
			
			for (int i = 0; i < names.Length; i++)
			{
				yield return new NameDef (names [i], new Value (() => values.GetValue (i)));
			}
		}
	}
}

