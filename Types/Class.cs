using System;
using System.Linq;
using System.Reflection;
using NOP.Collections;
using ExprList = NOP.Collections.List;

namespace NOP
{
	/// <summary>
	/// Class that contains dynamic properies and methods.
	/// </summary>
	public class Class : Module
	{
		public readonly Map<string, Member> Members;
		public readonly Map<string, Constructor> Constructors;
		
		public Class (Map<string, Definition> definitions, Map<string, Member> members) :
			base(definitions)
		{
			Members = members;
		}
		
		public Class (Type type) : base (type)
		{
			if (type.IsAbstract && type.IsSealed)
				throw new ArgumentException ("Type must be dynamic", "type");
			var bf = BindingFlags.Instance | BindingFlags.Public;
			Members = Map<string, Member>.FromPairs (
					type.GetMethods (bf).Select (
						mi => Tuple.Create<string, Member> (mi.Name, new Method (mi)))
				.Concat (
					type.GetProperties (bf).Select (
						pi => Tuple.Create<string, Member> (pi.Name, new Property (pi))))
				);
		}
	}
}