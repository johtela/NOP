namespace NOP
{
	using System;
	using Collections;
	using Grammar;
	
	/// <summary>
	/// TypeExpr class defines a simple typed lambda calculus that is used
	/// as an intermediate language for type checking. You can construct type
	/// expressions and then infer the type of the expression using the
	/// Hindley-Milner algorithm.
	/// </summary>
	public abstract class TypeExpr
	{
		private Expression _expression;
		public static Expression CurrentExpression;
		
		private TypeExpr ()
		{
			_expression = CurrentExpression;
		}
		
		/// <summary>
		/// Infers the principal the type of the expression. This function applies the rules 
		/// used to infer the type of the specific expression. 
		/// </summary>
		/// <returns>The substitution table that contains the type variables inferred.</returns>
		/// <param name='env'>The type environment used to determine the principal type.</param>
		/// <param name='expected'>The base type with which the expression type is unified.</param>
		/// <param name='subs'>The substitution table that is constructed so far.</param>
		public abstract void TypeCheck (TypeEnv env, MonoType expected);
		
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
				var type = Value.GetType ();
				return new MonoType.Con (new Name(type.Name, type.Namespace), StrictList<MonoType>.Empty);
			}
			
			public override void TypeCheck (TypeEnv env, MonoType expected)
			{
				CurrentExpression = _expression;
				MonoType.Unify (GetMonoType (), expected);
			}
		}

		/// <summary>
		/// Variables are symbols with type.
		/// </summary>
		private class Variable : TypeExpr
		{
			public readonly Name Name;
			
			public Variable (Name name)
			{
				Name = name;
			}
			
			public override void TypeCheck (TypeEnv env, MonoType expected)
			{
				CurrentExpression = _expression;
				if (!env.Contains (Name)) 
					throw new Exception (string.Format ("Name {0} not found", Name));
				var pt = env.Find (Name);
				MonoType.Unify (pt.Type.ApplySubs (), expected);
			}
		}
		
		/// <summary>
		/// Lambda expression defines a function of one argument. The argument can
		/// also be null, in which case the function is of type () -> 'a
		/// </summary>
		private class Lambda : TypeExpr
		{
			public readonly Name Argument;
			public readonly TypeExpr Body;
			
			public Lambda (Name arg, TypeExpr body)
			{
				Argument = arg;
				Body = body;
			}
			
			public override void TypeCheck (TypeEnv env, MonoType expected)
			{
				CurrentExpression = _expression;
				var a = Argument != null ? 
					MonoType.NewTypeVar () : 
					new MonoType.Con (new Name("Void", "System"));
				var b = MonoType.NewTypeVar ();
				
				MonoType.Unify (expected, new MonoType.Lam (a, b));
				if (Argument != null)
					env = env.Add (Argument, new Polytype (a, null));
				Body.TypeCheck (env, b);
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
			
			public override void TypeCheck (TypeEnv env, MonoType expected)
			{
				CurrentExpression = _expression;
				var a = Argument != null ? 
					MonoType.NewTypeVar () : 
					new MonoType.Con (new Name("Void", "System"));
				Function.TypeCheck (env, new MonoType.Lam (a, expected));
				if (Argument != null)
					Argument.TypeCheck (env, a);
			}
		}
		
		/// <summary>
		/// If expression checks that the condition evaluates to boolean type, and
		/// that the then and else expressions have the same type.
		/// </summary>
		private class IfElse : TypeExpr
		{
			public readonly TypeExpr Condition;
			public readonly TypeExpr ThenExpr;
			public readonly TypeExpr ElseExpr;
			
			public IfElse (TypeExpr cond, TypeExpr thenExpr, TypeExpr elseExpr)
			{
				Condition = cond;
				ThenExpr = thenExpr;
				ElseExpr = elseExpr;
			}
			
			public override void TypeCheck (TypeEnv env, MonoType expected)
			{
				CurrentExpression = _expression;
				var c = MonoType.NewTypeVar ();
				Condition.TypeCheck (env, c);
				MonoType.Unify (c, new MonoType.Con (new Name ("Boolean", "System")));
				
				var t = MonoType.NewTypeVar ();
				ThenExpr.TypeCheck (env, t);
				
				var e = MonoType.NewTypeVar ();
				ThenExpr.TypeCheck (env, e);
				
				MonoType.Unify (t, e);
				MonoType.Unify (t, expected);
			}
		}
		
		/// <summary>
		/// Let in expression defines a variable and an expression where the variable is
		/// bound to a value.
		/// </summary>
		private class LetIn : TypeExpr
		{
			public readonly Name VarName;
			public readonly TypeExpr Value;
			public readonly TypeExpr Body;
			
			public LetIn (Name variable, TypeExpr value, TypeExpr body)
			{
				VarName = variable;
				Value = value;
				Body = body;
			}
			
			public override void TypeCheck (TypeEnv env, MonoType expected)
			{
				CurrentExpression = _expression;
				MonoType a = MonoType.NewTypeVar ();
				Value.TypeCheck (env, a);
				MonoType t = a.ApplySubs ();
				Polytype newPt = new Polytype (t, t.GetTypeVars () - env.GetTypeVars ());
				Body.TypeCheck (env.Add (VarName, newPt), expected);
			}
		}
		
		/// <summary>
		/// Infers the the type of this expression using the specified type environment.
		/// </summary>
		public MonoType InferType (TypeEnv env)
		{
			MonoType.ClearSubs ();
			var a = MonoType.NewTypeVar ();
			TypeCheck (env, a);
			return a.ApplySubs ().RenameTypeVarsToLetters ();
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
			public static TypeExpr Var (Name name)
			{
				return new Variable (name);
			}

			public static TypeExpr Var (string name)
			{
				return Var (new Name (name));
			}

			/// <summary>
			/// Constuct a lambda expression.
			/// </summary>
			public static TypeExpr Lam (Name arg, TypeExpr body)
			{
				return new Lambda (arg, body);
			}

			public static TypeExpr Lam (string arg, TypeExpr body)
			{
				return Lam (new Name (arg), body);
			}

			/// <summary>
			/// Construct a lambda expression that has zero or more arguments.
			/// </summary>
			public static TypeExpr MultiLam (StrictList<Name> args, TypeExpr body)
			{
				if (args.IsEmpty)
					return Lam (null, body);
				else
				if (args.Rest.IsEmpty)
					return Lam (args.First, body);
				else
					return Lam (args.First, MultiLam (args.Rest, body));
			}

			public static TypeExpr MultiLam (StrictList<string> args, TypeExpr body)
			{
				return MultiLam (args.Map (a => new Name (a)), body);
			}
			
			/// <summary>
			/// Construct an function application.
			/// </summary>
			public static TypeExpr App (TypeExpr func, TypeExpr arg)
			{
				return new Application (func, arg);
			}
			
			/// <summary>
			/// Construct an if-then-else expression.
			/// </summary>
			public static TypeExpr If (TypeExpr cond, TypeExpr thenExpr, TypeExpr elseExpr)
			{
				return new IfElse (cond, thenExpr, elseExpr);
			}
			
			/// <summary>
			/// Construct a let-in expression.
			/// </summary>
			public static TypeExpr Let (Name variable, TypeExpr value, TypeExpr body)
			{
				return new LetIn (variable, value, body);
			}

			public static TypeExpr Let (string variable, TypeExpr value, TypeExpr body)
			{
				return Let (new Name (variable), value, body);
			}
		}
	}
}