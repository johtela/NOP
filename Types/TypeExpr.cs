namespace NOP
{
	using System;
	using Collections;
	
	/// <summary>
	/// TypeExpr class defines a simple typed lambda calculus that is used
	/// as an intermediate language for type checking. You can construct type
	/// expressions and then infer the type of the expression using the
	/// Hindley-Milner algorithm.
	/// </summary>
	public abstract class TypeExpr
	{
		/// <summary>
		/// Infers the principal the type of the expression. This function applies the rules 
		/// used to infer the type of the specific expression. 
		/// </summary>
		/// <returns>The substitution table that contains the type variables inferred.</returns>
		/// <param name='env'>The type environment used to determine the principal type.</param>
		/// <param name='baseType'>The base type with which the expression type is unified.</param>
		/// <param name='subs'>The substitution table that is constructed so far.</param>
		public abstract Substitution PrincipalType (TypeEnv env, MonoType baseType, 
		                                            Substitution subs);
		
		/// <summary>
		/// Literal expression. Can be any literal object.
		/// </summary>
		private class Literal : TypeExpr
		{
			public readonly object Value;
			
			public Literal (object value)
			{
				Value = value;
			}
			
			private MonoType GetMonoType ()
			{
				return new MonoType.Con (Value.GetType ().ToString (), List<MonoType>.Empty);
			}
			
			public override Substitution PrincipalType (TypeEnv env, MonoType baseType, 
			                                            Substitution subs)
			{
				return MonoType.MostGeneralUnifier (GetMonoType (), baseType, subs);
			}
		}

		/// <summary>
		/// Variables are symbols with type.
		/// </summary>
		private class Variable : TypeExpr
		{
			public readonly string Name;
			
			public Variable (string name)
			{
				Name = name;
			}
			
			public override Substitution PrincipalType (TypeEnv env, MonoType baseType, 
			                                            Substitution subs)
			{
				if (!env.Contains (Name)) 
					throw new Exception (string.Format ("Name {0} not found", Name));
				var pt = env.Find (Name);
				return MonoType.MostGeneralUnifier (pt.Type.ApplySubs (subs), baseType, subs);
			}
		}
		
		/// <summary>
		/// Lambda expression defines a function of one argument.
		/// </summary>
		private class Lambda : TypeExpr
		{
			public readonly string Argument;
			public readonly TypeExpr Body;
			
			public Lambda (string arg, TypeExpr body)
			{
				Argument = arg;
				Body = body;
			}
			
			public override Substitution PrincipalType (TypeEnv env, MonoType baseType, 
			                                            Substitution subs)
			{
				var a = MonoType.NewTypeVar ();
				var b = MonoType.NewTypeVar ();
				
				var s1 = MonoType.MostGeneralUnifier (baseType, new MonoType.Lam (a, b), subs);
				var newEnv = env.Add (Argument, new Polytype (a, null));
				return Body.PrincipalType (newEnv, b, s1);
			}
		}
		
		/// <summary>
		/// Function application invokes a lambda expression.
		/// </summary>
		private class Application : TypeExpr
		{
			public readonly TypeExpr Function;
			public readonly TypeExpr Argument;
			
			public Application (TypeExpr func, TypeExpr arg)
			{
				Function = func;
				Argument = arg;
			}
			
			public override Substitution PrincipalType (TypeEnv env, MonoType baseType, 
			                                            Substitution subs)
			{
				var a = MonoType.NewTypeVar ();
				var s1 = Function.PrincipalType (env, new MonoType.Lam (a, baseType), subs);
				return Argument.PrincipalType (env, a, s1);
			}
		}
		
		/// <summary>
		/// Let in expression defines a variable and an expression where the variable is
		/// bound to a value.
		/// </summary>
		private class LetIn : TypeExpr
		{
			public readonly string VarName;
			public readonly TypeExpr Value;
			public readonly TypeExpr Body;
			
			public LetIn (string variable, TypeExpr value, TypeExpr body)
			{
				VarName = variable;
				Value = value;
				Body = body;
			}
			
			public override Substitution PrincipalType (TypeEnv env, MonoType baseType, 
			                                            Substitution subs)
			{
				MonoType a = MonoType.NewTypeVar ();
				Substitution s1 = Value.PrincipalType (env, a, subs);
				MonoType t = a.ApplySubs (s1);
				Polytype newPt = new Polytype (t, t.GetTypeVars () - env.GetTypeVars ());
				return Body.PrincipalType (env.Add (VarName, newPt), baseType, s1);
			}
		}
		
		/// <summary>
		/// Returns the the type of this expression.
		/// </summary>
		public MonoType GetExprType (TypeEnv env)
		{
			var a = MonoType.NewTypeVar ();
			var emptySubs = Substitution.Empty;
			var s1 = PrincipalType (env, a, emptySubs);
			return a.ApplySubs (s1).RenameTypeVarsToLetters ();
		}
	
		/// <summary>
		/// Builder class can be inherited to bring in handy helper functions to the 
		/// same lexical context.
		/// </summary>
		public class Builder
		{
			/// <summary>
			/// Construct a literal.
			/// </summary>
			public static TypeExpr Lit (object value)
			{
				return new Literal (value);
			}
			
			/// <summary>
			/// Construct a variable with the specified name.
			/// </summary>
			public static TypeExpr Var (string name)
			{
				return new Variable (name);
			}
			
			/// <summary>
			/// Constuct a lambda expression.
			/// </summary>
			public static TypeExpr Lam (string arg, TypeExpr body)
			{
				return new Lambda (arg, body);
			}
			
			/// <summary>
			/// Construct an function application.
			/// </summary>
			public static TypeExpr App (TypeExpr func, TypeExpr arg)
			{
				return new Application (func, arg);
			}
			
			/// <summary>
			/// Construct a let-in expression.
			/// </summary>
			public static TypeExpr Let (string variable, TypeExpr value, TypeExpr body)
			{
				return new LetIn (variable, value, body);
			}
		}
	}
}