namespace NOP
{
	using System;
	using Collections;
	using StrSeq=System.Collections.Generic.IEnumerable<string>;

	public class Polytype
	{
		public readonly MonoType Type;
		public readonly Set<string> TypeVars;
			
		public Polytype (MonoType type, StrSeq tvars)
		{
			Type = type;
			TypeVars = tvars != null ? Set<string>.Create (tvars) : Set<string>.Empty;
		}
			
		public Set<string> GetTypeVars ()
		{
			return Type.GetTypeVars () - TypeVars;
		}
	}
}