namespace NOP
{
	using System;
	using Collections;
	using Grammar;

	/// <summary>
	/// State that is carried along when type checking is performed.
	/// </summary>
	/// <typeparam name="T">Type of the satellite data carried with state.</typeparam>
	public class TCState<T>
	{
		public readonly TypeEnv Env;
		public readonly Substitution Subs;
		public readonly int LastVar;
		public readonly T Satellite;

		public TCState (TypeEnv env, Substitution subs, int lastVar, T satellite)
		{
			Env = env;
			Subs = subs;
			LastVar = lastVar;
			Satellite = satellite;
		}

		public TCState (TCState<T> old, TypeEnv env) :
			this (env, old.Subs, old.LastVar, old.Satellite) { }

		public TCState (TCState<T> old, Substitution subs) :
			this (old.Env, subs, old.LastVar, old.Satellite) { }
	}

	/// <summary>
	/// Infers the principal the type of the expression. This function applies the rules 
	/// used to infer the type of the specific expression. 
	/// </summary>
	/// <typeparam name="T">The type of the satellite data carried with type checking</typeparam>
	/// <param name="state">The state used within type check.</param>
	/// <param name='expected'>The base type with which the expression type is unified.</param>
	/// <returns>The new state after the type check.</returns>
	public delegate TCState<T> TypeCheck<T> (TCState<T> state, MonoType expected);

	/// <summary>
	/// TypeCheck class defines a simple typed lambda calculus that is used
	/// as an intermediate language for type checking. You can construct type
	/// expressions and then infer the type of the expression using the
	/// Hindley-Milner algorithm W.
	/// </summary>
	public class TypeCheck
	{
		public static TypeCheck<T> Sequence<T> (TypeCheck<T> tc, TypeCheck<T> other)
		{
			return (st, exp) => other (tc (st, exp), exp);
		}

		public static TypeCheck<T> SetState<T> (TCState<T> state)
		{
			return (st, exp) => state;
		}

		/// <summary>
		/// Literal expression. Can be any literal object.
		/// </summary>
		public static TypeCheck<T> Literal<T> (object value)
		{
			var type = value.GetType ();
			return (st, exp) =>
			{
				MonoType.Unify (new MonoType.Con (type.Name, StrictList<MonoType>.Empty), exp);
				return st;
			};
		}

		/// <summary>
		/// Variables are symbols with type.
		/// </summary>
		public static TypeCheck<T> Variable<T> (string name)
		{
			return (st, exp) =>
			{
				if (!st.Env.Contains (name)) 
					throw new Exception (string.Format ("Name {0} not found", name));
				var pt = st.Env.Find (name);
				MonoType.Unify (pt.MonoType.ApplySubs (), exp);
				return st;
			};
		}

		/// <summary>
		/// Lambda expression defines a function of one argument. The argument can
		/// also be null, in which case the function is of type () -> 'a
		/// </summary>
		public static TypeCheck<T> Lambda<T> (string arg, TypeCheck<T> body)
		{
			return (st, exp) =>
			{
				var a = arg != null ? 
					MonoType.NewTypeVar () :
					new MonoType.Con ("Void");
				var b = MonoType.NewTypeVar ();
				
				MonoType.Unify (exp, new MonoType.Lam (a, b));
				if (arg != null)
					st = new TCState<T> (st, st.Env.Add (arg, new Polytype (a)));
				return body (st, b);
			};
		}

		/// <summary>
		/// Function application checks the type of argument and return type.
		/// </summary>
		public static TypeCheck<T> Application<T> (TypeCheck<T> func, TypeCheck<T> arg)
		{
			return (st, exp) =>
			{
				var a = arg != null ? 
					MonoType.NewTypeVar () :
					new MonoType.Con ("Void");
				st = func (st, new MonoType.Lam (a, exp));
				return arg == null ? st : arg (st, a);
			};
		}			
		
		/// <summary>
		/// If expression checks that the condition evaluates to boolean type, and
		/// that the then and else expressions have the same type.
		/// </summary>
		public static TypeCheck<T> IfElse<T> (TypeCheck<T> cond, TypeCheck<T> thenExpr, 
			TypeCheck<T> elseExpr)
		{
			return (st, exp) =>
			{
				var c = MonoType.NewTypeVar ();
				st = cond (st, c);
				MonoType.Unify (c, new MonoType.Con ("Boolean"));

				var t = MonoType.NewTypeVar ();
				st = thenExpr (st, t);

				var e = MonoType.NewTypeVar ();
				st = elseExpr (st, e);

				MonoType.Unify (t, e);
				MonoType.Unify (t, exp);
				return st;
			};
		}
		
		/// <summary>
		/// Let in expression defines a variable and an expression where the variable is
		/// bound to a value.
		/// </summary>
		public static TypeCheck<T> LetIn<T> (string variable, TypeCheck<T> value, TypeCheck<T> body) 
		{
			return (st, exp) =>
			{
				MonoType a = MonoType.NewTypeVar ();
				st = value (st, a);
				MonoType t = a.ApplySubs ();
				Polytype newPt = new Polytype (t, t.GetTypeVars () - st.Env.GetTypeVars ());
				return body (new TCState<T> (st, st.Env.Add (variable, newPt)), exp);
			};
		}
		
		/// <summary>
		/// Construct a lambda expression that has zero or more arguments.
		/// </summary>
		public static TypeCheck<T> MultiLambda<T> (StrictList<string> args, TypeCheck<T> body)
		{
			if (args.IsEmpty)
				return Lambda<T> (null, body);
			else
			if (args.Rest.IsEmpty)
				return Lambda<T> (args.First, body);
			else
				return Lambda<T> (args.First, MultiLambda (args.Rest, body));
		}

		/// <summary>
		/// Infers the the type of this expression using the specified type environment.
		/// </summary>
		public static MonoType InferType<T> (TypeCheck<T> tc, TypeEnv env)
		{
			MonoType.ClearSubs ();
			var a = MonoType.NewTypeVar ();
			var st = tc (new TCState<T> (env, Substitution.Empty, 0, default(T)), a);
			return a.ApplySubs ().RenameTypeVarsToLetters ();
		}
	}
}