﻿namespace NOP
{
	using System;
	using System.Linq;
	using NOP.Collections;
	
	/// <summary>
	/// Unification error is thrown when two monotypes cannot be unified, i.e. when there
	/// is a typing error in the expression.
	/// </summary>
	public class UnificationError : Exception
	{
		public readonly MonoType Type1;
		public readonly MonoType Type2;
		public readonly Expression Expression;
			
		public UnificationError (MonoType type1, MonoType type2) 
				: base (string.Format ("Could not unify type {0} with {1}", type1, type2))
		{
			Type1 = type1;
			Type2 = type2;
			Expression = TypeExpr.CurrentExpression;
		}
	}
	
	/// <summary>
	/// Monotype represents type of an expression. It might be a type variable, 
	/// a function type or a constructed type (with or without type variables).
	/// </summary>
	public abstract class MonoType
	{
		private static int _lastVar;
		private static char _nextTVarLetter;
		private static Map<string, char> _tVarMap;
		private static Substitution _subs;
		
		/// <summary>
		/// Clears the substitution table.
		/// </summary>
		public static void ClearSubs ()
		{
			_subs = Substitution.Empty;
			_lastVar = 0;
		}
		
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
		
		/// <summary>
		/// Apply the substitutions to this type.
		/// </summary>
		public abstract MonoType ApplySubs ();
		
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
			
			public override MonoType ApplySubs ()
			{
				var t = _subs.Lookup (Name);
				return (t.Equals (this)) ? this : t.ApplySubs ();
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

			public Lam (MonoType argument, MonoType result)
			{
				Argument = argument;
				Result = result;
			}
			
			public override MonoType ApplySubs ()
			{
				return new Lam (Argument.ApplySubs (), Result.ApplySubs ());
			}
			
			public override Set<string> GetTypeVars ()
			{
				return Argument.GetTypeVars () - Result.GetTypeVars ();
			}
			
			protected override MonoType TypeVarsToLetters ()
			{
				return new Lam (Argument.TypeVarsToLetters (), Result.TypeVarsToLetters ());
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

			public Con (string name, StrictList<MonoType> typeArgs)
			{
				Name = name;
				TypeArgs = typeArgs;
			}
			
			public Con (string name) : this (name, StrictList<MonoType>.Empty)
			{
			}
			
			public override MonoType ApplySubs ()
			{
				return new Con (Name, TypeArgs.Map (t => t.ApplySubs ()));
			}
			
			public override Set<string> GetTypeVars ()
			{
				return TypeArgs.ReduceLeft (Set<string>.Empty, (s, t) => s + t.GetTypeVars ());
			}
			
			protected override MonoType TypeVarsToLetters ()
			{
				return new Con (Name, TypeArgs.Map (t => t.TypeVarsToLetters ()));
			}
			
			public override bool Equals (object obj)
			{
				var other = obj as Con;
				return other != null && other.Name == Name && other.TypeArgs.Equals (TypeArgs);
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
		public static void Unify (MonoType type1, MonoType type2)
		{
			var a = type1.ApplySubs ();
			var b = type2.ApplySubs ();
						
			if (a is Var && b is Var && a.Equals (b))
				return;
			
			var va = a as Var;
			if (va != null && !b.GetTypeVars ().Contains (va.Name))
			{ 
				_subs = _subs.Extend (va.Name, b);
				return;
			}
			var vb = b as Var;
			if (vb != null && !a.GetTypeVars ().Contains (vb.Name))
			{
				_subs = _subs.Extend (vb.Name, a);
				return;
			}
			var la = a as Lam;
			var lb = b as Lam;
			if (la != null && lb != null)
			{
				Unify (la.Result, lb.Result);
				Unify (la.Argument, lb.Argument);
				return;
			}
			var ca = a as Con;
			var cb = b as Con;
			if (ca != null && cb != null && ca.Name == cb.Name)
			{
				var taa = ca.TypeArgs;
				var tab = cb.TypeArgs;
				
				while (!taa.IsEmpty && !tab.IsEmpty)
				{
					Unify (taa.First, tab.First);
					taa = taa.Rest;
					tab = tab.Rest;
				}		
				if (taa.IsEmpty && tab.IsEmpty)
					return;
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
	
		public class Builder
		{
			/// <summary>
			/// Builder method for creating a type constant.
			/// </summary>
			public static MonoType Constant (string name)
			{
				return new Con (name);
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
			public static MonoType Lambda (StrictList<MonoType> parameters, MonoType result)
			{
				return  parameters.IsEmpty ? 
				result : 
				new Lam (parameters.First, Lambda (parameters.Rest, result));
			}
		}
	}
}