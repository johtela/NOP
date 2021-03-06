﻿namespace NOP
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NOP.Collections;
	using Grammar;
	using System.Reflection;
	
	/// <summary>
	/// Unification error is thrown when two monotypes cannot be unified, i.e. when there
	/// is a typing error in the expression.
	/// </summary>
	public class UnificationError : Exception
	{
		public readonly MonoType Type1;
		public readonly MonoType Type2;
			
		public UnificationError (MonoType type1, MonoType type2) 
				: base (string.Format ("Could not unify type {0} with {1}", type1, type2))
		{
			Type1 = type1;
			Type2 = type2;
		}
	}
	
	/// <summary>
	/// Monotype represents type of an expression. It might be a type variable, 
	/// a function type or a constructed type (with or without type variables).
	/// </summary>
	public abstract class MonoType
	{
		private static char _nextTVarLetter;
		private static Map<string, char> _tVarMap;
		private static int _lastVar;
		
		/// <summary>
		/// Map a generated type variable name (T1, T2, T3...) to a single letter (a, b, c...)
		/// </summary>
		private static string GetTVarLetter (string tVarName)
		{
			if (_tVarMap.Contains (tVarName))
				return _tVarMap [tVarName].ToString ();
			else
			{
				var nextLetter = _nextTVarLetter++;
				_tVarMap = _tVarMap.Add (tVarName, nextLetter);
				return nextLetter.ToString ();
			}
		}
		
		/// <summary>
		/// Generate a new unique type variable.
		/// </summary>
		public static MonoType NewTypeVar ()
		{
			return new Var ("T" + (++_lastVar).ToString ());
		}

		public static void InitLastVar ()
		{
			_lastVar = 0;
		}

		public MonoType (MemberInfo mi)
		{
			MemberInfo = mi;
		}

		/// <summary>
		/// The underlying System.Type object.
		/// </summary>
		public readonly MemberInfo MemberInfo;

		/// <summary>
		/// Apply the substitutions to this type.
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

			public Var (string name) : base(null)
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

			public Lam (MonoType argument, MonoType result, MemberInfo mi)
				: base (mi)
			{
				Argument = argument;
				Result = result;
			}

			public override MonoType ApplySubs (Substitution subs)
			{
				return new Lam (Argument.ApplySubs (subs), Result.ApplySubs (subs), MemberInfo);
			}
			
			public override Set<string> GetTypeVars ()
			{
				return Argument.GetTypeVars () - Result.GetTypeVars ();
			}
			
			protected override MonoType TypeVarsToLetters ()
			{
				return new Lam (Argument.TypeVarsToLetters (), Result.TypeVarsToLetters (), MemberInfo);
			}
			
			public override bool Equals (object obj)
			{
				var other = obj as Lam;
				return other != null && other.Argument.Equals (Argument) && 
					other.Result.Equals (Result);
			}
			
			public override int GetHashCode ()
			{
				return Argument.GetHashCode () ^ Result.GetHashCode ();
			}
			
			private string SurroundLambdas (MonoType type)
			{
				return type is Lam ? string.Format ("({0})", type) : type.ToString ();
			}
			
			public override string ToString ()
			{
				return string.Format ("{0} -> {1}", Argument, Result);
			}
		}
		
		/// <summary>
		/// Constant type that might have type arguments.
		/// </summary>
		public class Con : MonoType
		{
			public readonly string Name;
			public readonly StrictList<MonoType> TypeArgs;

			public Con (string name, StrictList<MonoType> typeArgs, MemberInfo mi)
				: base (mi)
			{
				Name = name;
				TypeArgs = typeArgs;
			}

			public override MonoType ApplySubs (Substitution subs)
			{
				return new Con (Name, TypeArgs.Map (t => t.ApplySubs (subs)), MemberInfo);
			}
			
			public override Set<string> GetTypeVars ()
			{
				return TypeArgs.ReduceLeft (Set<string>.Empty, (s, t) => s + t.GetTypeVars ());
			}
			
			protected override MonoType TypeVarsToLetters ()
			{
				return new Con (Name, TypeArgs.Map (t => t.TypeVarsToLetters ()), MemberInfo);
			}
			
			public override bool Equals (object obj)
			{
				var other = obj as Con;
				return other != null && other.Name.Equals (Name) && other.TypeArgs.Equals (TypeArgs);
			}
			
			public override int GetHashCode ()
			{
				return TypeArgs.ReduceLeft (Name.GetHashCode (), (h, t) => h ^ t.GetHashCode ());
			}
			
			public override string ToString ()
			{
				return TypeArgs.IsEmpty ? Name : string.Format ("{0}{1}", Name, TypeArgs.ToString ("<", ">", ", "));
			}
		}
		
		/// <summary>
		/// Determine the mosts the general unifier of two monotypes.
		/// </summary>
		/// <returns>The substitution table that contains the reduced type variables.</returns>
		/// <param name='type1'>The first monotype to be unified.</param>
		/// <param name='type2'>the second monotype to be unified.</param>
		/// <param name='subs'>The substitution table used.</param>
		public static Substitution Unify (MonoType type1, MonoType type2, Substitution subs)
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
				return Unify (la.Argument, lb.Argument, Unify (la.Result, lb.Result, subs));

			var ca = a as Con;
			var cb = b as Con;
			if (ca != null && cb != null && ca.Name.Equals (cb.Name))
			{
				var taa = ca.TypeArgs;
				var tab = cb.TypeArgs;
				
				while (!taa.IsEmpty && !tab.IsEmpty)
				{
					subs = Unify (taa.First, tab.First, subs);
					taa = taa.Rest;
					tab = tab.Rest;
				}		
				if (taa.IsEmpty && tab.IsEmpty)
					return subs;
			}
			throw new UnificationError (a, b);
		}
		
		/// <summary>
		/// Renames the type variables of the monotype to letters.
		/// </summary>
		public MonoType RenameTypeVarsToLetters ()
		{
			_tVarMap = Map<string, char>.Empty;
			_nextTVarLetter = 'a';	
			return TypeVarsToLetters ();
		}

		/// <summary>
		/// Generalize monotype to polytype by promoting free type variables in
		/// monotype to generic type parameters.
		/// </summary>
		public Polytype Generalize(Bindings env)
		{
			return new Polytype(this, GetTypeVars() - env.GetTypeVars());
		}
	
		public class Builder
		{
			/// <summary>
			/// Builder method for creating a type constant.
			/// </summary>
			public static MonoType Constant (string name, IEnumerable<MonoType> args = null, MemberInfo mi = null)
			{
				return new Con (name, 
					args == null ? StrictList<MonoType>.Empty : List.FromEnumerable (args), 
					mi);
			}

			/// <summary>
			/// Builder method for creating a type variable.
			/// </summary>
			public static MonoType Variable (string name)
			{
				return new Var (name);
			}
		
			/// <summary>
			/// Builder method for creating lambdas with multiple parameters.
			/// </summary>
			public static MonoType Lambda (StrictList<MonoType> parameters, MonoType result, 
				MemberInfo mi = null)
			{
				return  parameters.IsEmpty ? 
					result : 
					new Lam (parameters.First, Lambda (parameters.Rest, result, mi), mi);
			}
		}
	}
}