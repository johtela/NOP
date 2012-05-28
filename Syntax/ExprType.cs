namespace NOP
{
	using System;
	using NOP.Collections;
	 
    public abstract class ExprType
    {
		/// <summary>
		/// Substitution of type variables is a map from strings to ExprType.
		/// </summary>
		protected class Substitution
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
		protected abstract ExprType ApplySub (Substitution sub);
		
		/// <summary>
		/// Gets the type variables of the type.
		/// </summary>
//		protected abstract Set<string> GetTypeVars ();
		
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
			
			protected override ExprType ApplySub (Substitution sub)
			{
				return new Lam(Argument.ApplySub(sub), Result.ApplySub(sub));
			}
			
//			protected override Set<string> GetTypeVars ()
//			{
//				return 
//			}
			
			public override bool Equals (object obj)
			{
				var other = obj as Lam;
				return other != null && other.Argument.Equals(Argument) && 
					other.Result.Equals(Result);
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
		/// Type variable.
		/// </summary>
        public class Var : ExprType
        {
            public readonly string Name;

            public Var(string name)
            {
                Name = name;
            }
			
			protected override ExprType ApplySub (Substitution sub)
			{
				var t = sub.Lookup(Name);
				return (t.Equals(this)) ? this : t.ApplySub (sub);
			}
			
			public override bool Equals (object obj)
			{
				return Name.Equals(obj);
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
			
			protected override ExprType ApplySub (Substitution sub)
			{
				return new Con(Name, TypeArgs.Map(t => t.ApplySub (sub)));
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
			
		}
		
    }
}
