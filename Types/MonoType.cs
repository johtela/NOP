namespace NOP
{
	using System;
	using System.Linq;
	using NOP.Collections;

	public abstract class MonoType
    {
		private static int _lastVar;
		private static char _nextTVarLetter;
		private static Map<string, char> _tVarMap;
		
		private static string GetTVarLetter (string tVarName)
		{
			return _tVarMap.Contains (tVarName) ?
				_tVarMap[tVarName].ToString () :
				(_nextTVarLetter++).ToString ();
		}
		
		public static MonoType NewTypeVar ()
		{
			return new Var("T" + (++_lastVar).ToString ());
		}
		
		/// <summary>
		/// Apply the substitution to this type.
		/// </summary>
		public abstract MonoType ApplySubs (Substitution subs);
		
		/// <summary>
		/// Gets the type variables of the type.
		/// </summary>
		public abstract Set<string> GetTypeVars ();
		
		/// <summary>
		/// Renames the type variables of this type to letters.
		/// </summary>
		protected abstract MonoType TypeVarsToLetters ();
		
		/// <summary>
		/// Type variable.
		/// </summary>
		public class Var : MonoType
		{
			public readonly string Name;

			public Var (string name)
			{
				Name = name;
			}
			
			public override MonoType ApplySubs (Substitution subs)
			{
				var t = subs.Lookup (Name);
				return (t.Equals (this)) ? this : t.ApplySubs (subs);
			}
			
			public override Set<string> GetTypeVars ()
			{
				return Set<string>.Create (Name);
			}
			
			protected override MonoType TypeVarsToLetters ()
			{
				return new Var (GetTVarLetter (Name));
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
        public class Lam : MonoType
        {
            public readonly MonoType Argument, Result;

            public Lam(MonoType argument, MonoType result)
            {
                Argument = argument;
                Result = result;
            }
			
			public override MonoType ApplySubs (Substitution subs)
			{
				return new Lam (Argument.ApplySubs (subs), Result.ApplySubs (subs));
			}
			
			public override Set<string> GetTypeVars ()
			{
				return Argument.GetTypeVars () - Result.GetTypeVars ();
			}
			
			protected override MonoType TypeVarsToLetters ()
			{
				return new Lam (Argument.TypeVarsToLetters(), Result.TypeVarsToLetters ());
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
        public class Con : MonoType
        {
            public readonly string Name;
            public readonly List<MonoType> TypeArgs;

            public Con(string name, List<MonoType> typeArgs)
            {
                Name = name;
                TypeArgs = typeArgs;
            }
			
			public override MonoType ApplySubs (Substitution subs)
			{
				return new Con (Name, TypeArgs.Map (t => t.ApplySubs (subs)));
			}
			
			public override Set<string> GetTypeVars ()
			{
				return TypeArgs.Fold (Set<string>.Empty, (s, t) => s + t.GetTypeVars ());
			}
			
			protected override MonoType TypeVarsToLetters ()
			{
				return new Con (Name, TypeArgs.Map(t => t.TypeVarsToLetters ()));
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
		
		public static Substitution MostGeneralUnifier (MonoType type1, MonoType type2, 
		                                               Substitution subs)
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
		
		public MonoType RenameTypeVarsToLetters ()
		{
			_tVarMap = Map<string, char>.Empty;
			_nextTVarLetter = 'a';	
			return TypeVarsToLetters ();
		}
    }
}