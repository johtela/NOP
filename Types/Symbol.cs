using NOP.Collections;
using ExprList = NOP.Collections.List<object>;

namespace NOP
{
	/// <summary>
	/// Symbols used by the programs. 
	/// </summary>
	public sealed class Symbol
	{
		public readonly string Name;

		public Symbol (string name)
		{
			Name = name;
		}

		public override bool Equals (object obj)
		{
			return (obj is Symbol) && Name.Equals (obj);
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode ();
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}