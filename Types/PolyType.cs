namespace NOP
{
	using System;
	using Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	
	/// <summary>
	/// Polytypes represent generic types, from which a set of MonoTypes can be generated. They
	/// are also called type schemes. Polytypes have the form "forall v1,v2... t" where v1,v2... 
	/// are the generic type variables in monotype t.
	/// </summary>
	public class Polytype
	{
		/// <summary>
		/// The underlying monotype.
		/// </summary>
		public readonly MonoType MonoType;
		
		/// <summary>
		/// The generic (unbound) type variables in mono type.
		/// </summary>
		public readonly Set<string> GenericTypeVars;

		/// <summary>
		/// The System.Type corresponding to the polytype.
		/// </summary>
		public readonly MemberInfo MemberInfo;

		/// <summary>
		/// Create a polytype.
		/// </summary>
		public Polytype (MonoType monoType, MemberInfo mi, IEnumerable<string> tvars)
		{
			MonoType = monoType;
			MemberInfo = mi;
			GenericTypeVars = tvars != null ? Set<string>.Create (tvars) : Set<string>.Empty;
		}

		public Polytype (MonoType monoType, MemberInfo mi, params string[] tvars) : 
			this (monoType, mi, (IEnumerable<string>)tvars) { }

		public Polytype (MonoType monoType, IEnumerable<string> tvars) :
			this (monoType, null, (IEnumerable<string>)tvars) { }

		public Polytype (MonoType monoType, params string[] tvars) :
			this (monoType, null, (IEnumerable<string>)tvars) { }

		/// <summary>
		/// Gets the free type variables.
		/// </summary>
		public Set<string> GetTypeVars ()
		{
			return MonoType.GetTypeVars () - GenericTypeVars;
		}

        /// <summary>
        /// Create a monotype by instantiating the generic type parameters 
        /// with fresh type variables.
        /// </summary>
        public MonoType Instantiate ()
        {
            var pairs = from tv in GenericTypeVars
                        select Tuple.Create (tv, MonoType.NewTypeVar ());
            var subs = new Substitution (Map<string, MonoType>.FromPairs (pairs));
            return MonoType.ApplySubs (subs);
        }
		
		public override string ToString ()
		{
			return string.Format ("{0} {1}", MonoType, GenericTypeVars);
		}
	}
}