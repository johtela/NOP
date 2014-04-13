namespace NOP
{
	using System;
	using Collections;
	using Grammar;

	/// <summary>
	/// State that is carried along when type checking is performed.
	/// </summary>
	public class TCState
	{
		public readonly TypeEnv Env;
		public readonly Substitution Subs;

		public TCState (TypeEnv env, Substitution subs)
		{
			Env = env;
			Subs = subs;
		}

		public TCState (TCState old, TypeEnv env) :
			this (env, old.Subs) { }

		public TCState (TCState old, Substitution subs) :
			this (old.Env, subs) { }
	}

	/// <summary>
	/// Infers the principal the type of the expression. This function applies the rules 
	/// used to infer the type of the specific expression. 
	/// </summary>
	/// <typeparam name="T">The type of the satellite data carried with type checking</typeparam>
	/// <param name="state">The state used within type check.</param>
	/// <param name='expected'>The base type with which the expression type is unified.</param>
	/// <returns>The new state after the type check.</returns>
	public delegate TCState TypeCheck (TCState state, MonoType expected);

	/// <summary>
	/// TypeCheck class defines a simple typed lambda calculus that is used
	/// as an intermediate language for type checking. You can construct type
	/// expressions and then infer the type of the expression using the
	/// Hindley-Milner algorithm W.
	/// </summary>
	public class TC
	{
		public static TypeCheck DoAfter (TypeCheck tc, Action<TCState, MonoType> action)
		{
			return (st, exp) =>
			{
				st = tc (st, exp);
				action (st, exp);
				return st;
			};
		}

		/// <summary>
		/// Literal expression. Can be any literal object.
		/// </summary>
		public static TypeCheck Literal (object value)
		{
			var type = value.GetType ();
			return (st, exp) => new TCState (st, 
				MonoType.Unify (new MonoType.Con (type.Name, StrictList<MonoType>.Empty), exp, st.Subs));
		}

		/// <summary>
		/// Variables are symbols with type.
		/// </summary>
		public static TypeCheck Variable (string name)
		{
			return (st, exp) =>
			{
				if (!st.Env.Contains (name)) 
					throw new Exception (string.Format ("Name {0} not found", name));
				var pt = st.Env.Find (name);
				return new TCState (st, 
					MonoType.Unify (pt.Instantiate ().ApplySubs (st.Subs), exp, st.Subs));
			};
		}

		/// <summary>
		/// Lambda expression defines a function of one argument. The argument can
		/// also be null, in which case the function is of type () -> 'a
		/// </summary>
		public static TypeCheck Lambda (string arg, TypeCheck body)
		{
			return (st, exp) =>
			{
				var subs = st.Subs;
				var env = st.Env;
				var a = arg != null ? 
					MonoType.NewTypeVar () :
					new MonoType.Con ("Void");
				var b = MonoType.NewTypeVar ();
				
				subs = MonoType.Unify (exp, new MonoType.Lam (a, b), subs);
				if (arg != null)
					env = st.Env.Add (arg, new Polytype (a));
				return body (new TCState (env, subs), b);
			};
		}

		/// <summary>
		/// Function application checks the type of argument and return type.
		/// </summary>
		public static TypeCheck Application (TypeCheck func, TypeCheck arg)
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
		public static TypeCheck IfElse (TypeCheck cond, TypeCheck thenExpr, 
			TypeCheck elseExpr)
		{
			return (st, exp) =>
			{
				var c = MonoType.NewTypeVar ();
				st = cond (st, c);
				st = new TCState (st, MonoType.Unify (c, new MonoType.Con ("Boolean"), st.Subs));

				var t = MonoType.NewTypeVar ();
				st = thenExpr (st, t);

				var e = MonoType.NewTypeVar ();
				st = elseExpr (st, e);

				return new TCState (st, MonoType.Unify (t, exp, MonoType.Unify (t, e, st.Subs)));
			};
		}
		
		/// <summary>
		/// Let in expression defines a variable and an expression where the variable is
		/// bound to a value.
		/// </summary>
		public static TypeCheck LetIn (string variable, TypeCheck value, TypeCheck body) 
		{
			return (st, exp) =>
			{
				var a = MonoType.NewTypeVar ();
				st = value (st, a);
				var newPt = a.ApplySubs (st.Subs).Generalize (st.Env);
				return body (new TCState (st, st.Env.Add (variable, newPt)), exp);
			};
		}

		/// <summary>
		/// Letrec in expression defines a variable which is already in scope in the definition
		/// of the variable. With that, recursive definitions can be created. The expression
		/// can consist of multiple recursive let definitions. Instead of single name and value 
		/// expression, the function takes a list of (name, value) pairs. This ability is needed 
		/// for defining mutually recursive functions or variables.
		/// </summary>
		public static TypeCheck LetRec (StrictList<Tuple<string, TypeCheck>> definitions, TypeCheck body)
		{
			return (st, exp) =>
			{
				var tvars = definitions.Map (_ => MonoType.NewTypeVar ());
				var newEnv = definitions.ReduceWith (st.Env, tvars, 
					(e, def, tv) => e.Add (def.Item1, new Polytype (tv)));
				st = definitions.ReduceWith (new TCState (st, newEnv), tvars,
					(s, def, tv) => def.Item2 (s, tv));
				newEnv = definitions.ReduceWith(newEnv, tvars,
					(e, def, tv) => e.Replace (def.Item1, tv.ApplySubs (st.Subs).Generalize (e)));
				return body (new TCState (st, newEnv), exp);
			};
		}

		/// <summary>
		/// Construct a lambda expression that has zero or more arguments.
		/// </summary>
		public static TypeCheck MultiLambda (StrictList<string> args, TypeCheck body)
		{
			if (args.IsEmpty)
				return Lambda (null, body);
			else
			if (args.Rest.IsEmpty)
				return Lambda (args.First, body);
			else
				return Lambda (args.First, MultiLambda (args.Rest, body));
		}

		/// <summary>
		/// Infers the the type of this expression using the specified type environment.
		/// </summary>
		public static MonoType InferType (TypeCheck tc, TypeEnv env)
		{
			MonoType.InitLastVar ();
			var a = MonoType.NewTypeVar ();
			var st = tc (new TCState (env, Substitution.Empty), a);
			return a.ApplySubs (st.Subs).RenameTypeVarsToLetters ();
		}
	}
}