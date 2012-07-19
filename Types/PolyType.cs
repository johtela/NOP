namespace NOP
{
	using System;
	using Collections;
	using StrSeq=System.Collections.Generic.IEnumerable<string>;
	
	/// <summary>
	/// Polytype have a form "forall v1,v2... t" where v1,v2... are the free type
	/// variables in monotype t. Polytypes are used to unify type variables that are not yet
	/// bound to a monotype in the inference process.
	/// </summary>
	public class Polytype
	{
		/// <summary>
		/// The underlying monotype.
		/// </summary>
		public readonly MonoType Type;
		
		/// <summary>
		/// The free type variables in mono type.
		/// </summary>
		public readonly Set<string> TypeVars;
			
		public Polytype (MonoType type, StrSeq tvars)
		{
			Type = type;
			TypeVars = tvars != null ? Set<string>.Create (tvars) : Set<string>.Empty;
		}
			
		/// <summary>
		/// Gets the free type variables.
		/// </summary>
		public Set<string> GetTypeVars ()
		{
			return Type.GetTypeVars () - TypeVars;
		}
	}
}