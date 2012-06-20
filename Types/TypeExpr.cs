namespace NOP
{
	using System;
	using Collections;

	public abstract class TypeExpr
	{
		public abstract Substitution PrincipalType (TypeEnv env, MonoType baseType, 
		                                            Substitution subs);

		private class Literal : TypeExpr
		{
			public readonly object Value;
			
			public Literal (object value)
			{
				Value = value;
			}
			
			private MonoType GetMonoType()
			{
				return new MonoType.Con (Value.GetType().ToString(), List<MonoType>.Empty);
			}
			
			public override Substitution PrincipalType (TypeEnv env, MonoType baseType, 
			                                            Substitution subs)
			{
				return MonoType.MostGeneralUnifier (GetMonoType (), baseType, subs);
			}
		}
		
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
						
		public MonoType GetExprType (TypeEnv env)
		{
			var a = MonoType.NewTypeVar ();
			var emptySubs = Substitution.Empty;
			var s1 = PrincipalType (env, a, emptySubs);
			return a.ApplySubs (s1).RenameTypeVarsToLetters ();
		}
	
		public class Builder
		{
			public static TypeExpr Lit (object value)
			{
				return new Literal (value);
			}
			
			public static TypeExpr Var (string name)
			{
				return new Variable (name);
			}
			
			public static TypeExpr Lam (string arg, TypeExpr body)
			{
				return new Lambda (arg, body);
			}
			
			public static TypeExpr App (TypeExpr func, TypeExpr arg)
			{
				return new Application (func, arg);
			}
			
			public static TypeExpr Let (string variable, TypeExpr value, TypeExpr body)
			{
				return new LetIn (variable, value, body);
			}
		}
	}
}