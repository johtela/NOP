namespace NOP
{
	using System;
	using System.Linq;
	using NOP.Collections;
	using StrSeq=System.Collections.Generic.IEnumerable<string>; 
	 
    public abstract class ExprType
    {
		/// <summary>
		/// Substitution of type variables is a map from strings to ExprType.
		/// </summary>
		public class Substitution
		{
			private readonly Map<string, ExprType> _map;
			
			private Substitution (Map<string, ExprType> map)
			{
				_map = map;
			}
			
			public Substitution Extend (string name, ExprType type)
			{
				return new Substitution(_map.Add (name, type));
			}
			
			public ExprType Lookup (string name)
			{
				return _map.Contains (name) ? _map[name] : new Var(name);
			}
		}
		
		/// <summary>
		/// Apply the substitution to this type.
		/// </summary>
		protected abstract ExprType ApplySubs (Substitution subs);
		
		/// <summary>
		/// Gets the type variables of the type.
		/// </summary>
		protected abstract Set<string> GetTypeVars ();
		
		/// <summary>
		/// Type variable.
		/// </summary>
		public class Var : ExprType
		{
			public readonly string Name;

			public Var (string name)
			{
				Name = name;
			}
			
			protected override ExprType ApplySubs (Substitution subs)
			{
				var t = subs.Lookup (Name);
				return (t.Equals (this)) ? this : t.ApplySubs (subs);
			}
			
			protected override Set<string> GetTypeVars ()
			{
				return Set<string>.Create (Name);
			}
			
			public override bool Equals (object obj)
			{
				var other = obj as Var;
				return other != null && Name.Equals (other.Name);
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
		
		/// <summary>
		/// Lambda type, Argument -> Result
		/// </summary>
        public class Lam : ExprType
        {
            public readonly ExprType Argument, Result;

            private Lam(ExprType argument, ExprType result)
            {
                Argument = argument;
                Result = result;
            }
			
			protected override ExprType ApplySubs (Substitution subs)
			{
				return new Lam (Argument.ApplySubs (subs), Result.ApplySubs (subs));
			}
			
			protected override Set<string> GetTypeVars ()
			{
				return Argument.GetTypeVars () - Result.GetTypeVars ();
			}
			
			public override bool Equals (object obj)
			{
				var other = obj as Lam;
				return other != null && other.Argument.Equals (Argument) && 
					other.Result.Equals (Result);
			}
			
			public override int GetHashCode ()
			{
				return Argument.GetHashCode() ^ Result.GetHashCode();
			}
			
			public override string ToString ()
		{
			return string.Format ("{0} -> {1}", Argument, Result);
		}
        }
		
		/// <summary>
		/// Constructed type that has generic arguments.
		/// </summary>
        public class Con : ExprType
        {
            public readonly string Name;
            public readonly List<ExprType> TypeArgs;

            private Con(string name, List<ExprType> typeArgs)
            {
                Name = name;
                TypeArgs = typeArgs;
            }
			
			protected override ExprType ApplySubs (Substitution subs)
			{
				return new Con (Name, TypeArgs.Map (t => t.ApplySubs (subs)));
			}
			
			protected override Set<string> GetTypeVars ()
			{
				return TypeArgs.Fold (Set<string>.Empty, (s, t) => s + t.GetTypeVars ());
			}
			
			public override bool Equals (object obj)
			{
				var other = obj as Con;
				return other != null && other.Name == Name && other.TypeArgs.Equals(TypeArgs);
			}
			
			public override int GetHashCode ()
			{
				return TypeArgs.Fold (Name.GetHashCode (), (h, t) => h ^ t.GetHashCode ());
			}
        }
		
		public class Polytype
		{
			public readonly ExprType Type;
			public readonly Set<string> TypeVars;
			
			public Polytype (ExprType type, StrSeq tvars)
			{
				Type = type;
				TypeVars = Set<string>.Create (tvars);
			}
			
			public Set<string> GetTypeVars ()
			{
				return Type.GetTypeVars () - TypeVars;
			}
		}
		
		public class Env 
		{
			private readonly Map<string, Polytype> _map;
			
			private Env (Map<string, Polytype> map)
			{
				_map = map;
			}
		
			public Polytype Find (string name)
			{
				return _map [name];
			}
			
			public bool Contains (string name)
			{
				return _map.Contains (name);
			}
			
			public Env Add (string name, Polytype type)
			{
				return new Env (_map.Add (name, type));
			}
			
			public Set<string> GetTypeVars ()
			{
				return _map.Values.Aggregate (Set<string>.Empty, (s, pt) => s + pt.GetTypeVars ());
			}
		}
		
		public class UnificationError : Exception
		{
			public readonly ExprType Type1;
			public readonly ExprType Type2;
			
			public UnificationError (ExprType type1, ExprType type2) 
				: base (string.Format ("Could not unify type {0} with {1}", type1, type2))
			{
				Type1 = type1;
				Type2 = type2;
			}
		}
		
		public Substitution MostGeneralUnifier (ExprType type1, ExprType type2, Substitution subs)
		{
			var a = type1.ApplySubs (subs);
			var b = type2.ApplySubs (subs);
						
			if (a is Var && b is Var && a.Equals (b))
				return subs;
			
			var va = a as Var;
			if (va != null && !b.GetTypeVars ().Contains (va.Name))
				return subs.Extend (va.Name, b);
			
			var vb = b as Var;
			if (vb != null && !a.GetTypeVars ().Contains (vb.Name))
				return subs.Extend (vb.Name, a);
			
			var la = a as Lam;
			var lb = b as Lam;
			if (la != null && lb != null)
				return MostGeneralUnifier (la.Argument, lb.Argument, 
				                           MostGeneralUnifier (la.Result, lb.Result, subs));
			
			var ca = a as Con;
			var cb = b as Con;
			if (ca != null && cb != null && ca.Name == cb.Name)
				return ca.TypeArgs.FoldWith (subs, (s, t1, t2) => MostGeneralUnifier (t1, t2, s), cb.TypeArgs);
		
			throw new UnificationError (a, b);
		}
    }
}
