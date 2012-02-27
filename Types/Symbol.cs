namespace NOP
{
	using System;

	public class Symbol : SExpr
	{
		public readonly string Name;
		
		public Symbol (string name)
		{
			Name = name;
		}
		
		public override bool Equals (object obj)
		{
			var other = obj as Symbol;
			return other != null && other.Name == Name;
		}
		
		public override int GetHashCode ()
		{
			return Name.GetHashCode ();
		}
	}
}

