namespace NOP
{
	// TODO: Change the argument of the interpreter error to specify the location.
	// TODO: Transform the special forms (lists starting with symbols "if", "lambda", etc.)
	// 		 to their own classes.
	
	using System;
	using NOP.Collections;
	using ExprList = NOP.Collections.List<object>;

	/// <summary>
	/// Exception for interpreter errors.
	/// </summary>
	public class InterpreterException : Exception
	{
		public readonly object Expression;

		public InterpreterException (object expr, string message) : base(message)
		{
			Expression = expr;
		}
	}

	/// <summary>
	/// Result of the interpreter evaluation.
	/// </summary>
	internal struct EvalResult
	{
		public readonly Environment Env;
		public readonly object Result;

		public EvalResult (Environment env, object result)
		{
			Env = env;
			Result = result;
		}
	}

	/// <summary>
	/// The interpreter for evaluating expressions.
	/// </summary>
	public static class Interpreter
	{
		/// <summary>
		/// An interpretation error with given expression.
		/// </summary>
		static internal void Error (object expr, string message)
		{
			throw new InterpreterException (expr, message);
		}

		/// <summary>
		/// An interpretation error with no expression given.
		/// </summary>
		static internal void Error (string message)
		{
			throw new InterpreterException (null, message);
		}

		/// <summary>
		/// Evaluate an expression with an empty environment.
		/// </summary>
		/// <param name="expr">The expression to be evaluated.</param>
		/// <returns>The value of the expression.</returns>
		public static object Evaluate (object expr)
		{
			return Eval (new Environment (), expr).Result;
		}
		
		/// <summary>
		/// Evaluate a list of expressions extending the environment as we go.
		/// </summary>
		/// <param name="exprs">The expressions to be evaluated.</param>
		/// <returns>The result of the last expression in the list.</returns>
		public static object Evaluate (ExprList exprs)
		{
			var result = new EvalResult (new Environment (), null);
			
			for (var list = exprs; !list.IsEmpty; list = list.Rest)
				result = Eval (result.Env, list.First);
			return result.Result;
		}

		/// <summary>
		/// Evaluate an expression with given environment.
		/// </summary>
		/// <param name="env">The environment to be used for evaluation.</param>
		/// <param name="expr">The expression to be evaluated.</param>
		/// <returns>The new envonment and the result of the expression.</returns>
		static internal EvalResult Eval (Environment env, object expr)
		{
			// Is this a symbol?
			if (expr is Symbol)
				return new EvalResult (env, env.Lookup ((expr as Symbol).Name));
			// Or is it a list?
			if (expr is ExprList)
			{
				var list = expr as ExprList;
				if (list.IsEmpty)
					return new EvalResult (env, ExprList.Empty);
				var symbol = list.First as Symbol;
				if (symbol != null)
				{
					// Check if we have any of the special forms as first item.
					switch (symbol.Name)
					{
						case "quote":
							return new EvalResult (env, list.Rest);
						case "if":
							return EvalIf (env, list.Rest);
						case "begin":
							return EvalBegin (env, list.Rest);
						case "define":
							return EvalDefine (env, list.Rest);
						case "lambda":
							return MakeFunction (env, list.Rest);
						case "set!":
							return EvalSet (env, list.Rest);
						default:
							return InvokeFunction (env, symbol, list.Rest);
					}
				}
				// Is this an external function call?
				var func = list.First as Function;
				if (func != null)
					return InvokeFunction (env, func, list.Rest);
				// Or is it a method call or property read?
				var obj = list.First;
				var rest = list.Rest;
				if (!rest.IsEmpty)
				{
					if (rest.First is Method)
						return InvokeMethod (env, obj, rest.First as Method, rest.Rest);
					else
					if (rest.First is Property)
						return GetProperty (env, obj, rest.First as Property);
				}
				// Otherwise throw an error.
				Error (rest.First, "Expected a function or method call, or property read.");
			}
			var val = expr as Value;
			if (val != null)
				return new EvalResult (env, val.Get ());
			return new EvalResult (env, expr);
		}

		/// <summary>
		/// Symbol definition evaluation.
		/// </summary>
		static internal EvalResult EvalDefine (Environment env, ExprList exprs)
		{
			var symbol = Expect<Symbol>(ref exprs, "symbol");
			var val = Expect<object>(ref exprs, "right hand side of definition clause");
			var res = Eval (env, val).Result;
			return new EvalResult (env.Define (symbol.Name, res), res);
		}

		/// <summary>
		/// Evaluate an if expression.
		/// </summary>
		static internal EvalResult EvalIf (Environment env, ExprList exprs)
		{
			if (exprs.Length != 3)
				Error (exprs.First, "Invalid number of expressions in an 'if' clause");
			var condRes = Eval (env, exprs.First);
			if ((bool)condRes.Result)
				return new EvalResult (env, Eval (condRes.Env, exprs.Nth (1)).Result);
			else
				return new EvalResult (env, Eval (condRes.Env, exprs.Nth (2)).Result);
		}

		/// <summary>
		/// Evaluate a begin sequence expression.
		/// </summary>
		static internal EvalResult EvalBegin (Environment env, ExprList exprs)
		{
			if (exprs.IsEmpty)
				Error ("Expected at least one expression");
			
			var res = new EvalResult (env, null);
			do
			{
				res = Eval (res.Env, exprs.First);
				exprs = exprs.Rest;
			}
			while (!exprs.IsEmpty);
			return new EvalResult (env, res.Result);
		}

		/// <summary>
		/// Bind parameters values to their corresponding symbols.
		/// </summary>
		/// <param name="values">The parameter values given.</param>
		/// <returns>The updated environment that has the parameters defined.</returns>
		private static Environment BindParams (Environment env, List<string> names, ExprList values)
		{
			while (true)
			{
				if (names.IsEmpty)
				{
					if (values.IsEmpty)
						return env;
					else
						Interpreter.Error (values.First, "Too many parameters");
				}
				if (values.IsEmpty)
					Interpreter.Error (string.Format ("Parameter '{0}' is missing", names.First));
				if (names.First == ".")
				{
					return env.Define (names.Rest.First, values);
				}
				env = env.Define (names.First, values.First);
				names = names.Rest;
				values = values.Rest;
			}
		}

		/// <summary>
		/// Create a function.
		/// </summary>
		private static EvalResult MakeFunction (Environment env, ExprList definition)
		{
			var list = Expect<ExprList>(ref definition, "list of parameters");
			var parameters = list.Map (expr =>
			{
				var sym = expr as Symbol;
				if (sym == null)
					Interpreter.Error (expr, "Expected a parameter name");
				return sym.Name;
			});
			var dot = parameters.FindNext (".");
			if (!dot.IsEmpty && dot.Length != 2)
				Interpreter.Error ("There should be only one parameter after '.'");
			if (definition.IsEmpty)
				Interpreter.Error (definition, "Function body is missing");
			return new EvalResult (env, new Func (
				args => EvalBegin (BindParams (env, parameters, args), definition).Result));
		}

		/// <summary>
		/// Invoke a function defined by locally and represented by symbol.
		/// </summary>
		private static EvalResult InvokeFunction (Environment env, Symbol fname, ExprList args)
		{
			var func = env.Lookup (fname.Name) as Func;
			if (func == null)
				Error (fname, "Expected a function");
			return new EvalResult (env, func (EvaluateArguments (env, args)));
		}
		
		/// <summary>
		/// Invokes a function defined externally and represented by Function object.
		/// </summary>
		private static EvalResult InvokeFunction (Environment env, Function function, ExprList args)
		{
			return new EvalResult (env, function.Call (EvaluateArguments (env, args)));
		}
		
		/// <summary>
		/// Calls a method defined externally.
		/// </summary>
		private static EvalResult InvokeMethod (Environment env, object obj, Method method, 
			ExprList args)
		{
			CheckIsMember(method, obj);
			return new EvalResult (env, method.Call (obj, EvaluateArguments (env, args)));
		}
		
		/// <summary>
		/// Gets the value of a property.
		/// </summary>
		private static EvalResult GetProperty (Environment env, object obj, Property prop)
		{
			CheckIsMember (prop, obj);
			return new EvalResult (env, prop.Get (obj));
		}

		/// <summary>
		/// Evaluates the arguments of a function or method call.
		/// </summary>
		private static ExprList EvaluateArguments (Environment env, ExprList args)
		{
			return args.Map (expr => Eval (env, expr).Result);
		}
		
		/// <summary>
		/// Evaluates a set! assignment expression.
		/// </summary>
		private static EvalResult EvalSet (Environment env, ExprList exprs)
		{
			Property prop = null;
			object obj = null;	
			Variable variable;
			
			if (!NextToken<Variable>(ref exprs, out variable))
			{
				obj = Expect<object>(ref exprs, "object");
				prop = Expect<Property>(ref exprs, "property");
				CheckIsMember (prop, obj); 
			}
			var val = Expect<object>(ref exprs, "right hand side of assignment clause");
			if (prop != null) prop.Set (obj, val);
			else variable.Set (val);
			return new EvalResult (env, val);
		}
		
		/// <summary>
		/// A helper function to get the next token in the list. If the list is exhausted
		/// or the type of the token does not match, an error is raised. Advances the
		/// list of expressions to the next item.
		/// </summary>
		private static T Expect<T>(ref ExprList exprs, string token) where T: class
		{
			if (exprs.IsEmpty)
				Error (exprs, string.Format ("Expected {0} but reached end of list.", token));
			var obj = exprs.First as T;
			if (obj == null)
				Error (exprs, string.Format ("Expected {0} but got {1}.", token, exprs.First));
			exprs = exprs.Rest;
			return obj;
		}
		
		/// <summary>
		/// Checks if the next token in the list is of given type. Raises an error
		/// if the list is exhausted. If the specified token is read the list is
		/// advanced to the next item. Otherwise the list remains the same.
		/// </summary>
		private static bool NextToken<T>(ref ExprList exprs, out T token) where T: class
		{
			if (exprs.IsEmpty)
				Error (exprs, string.Format ("Unexpected end of list."));
			token = exprs.First as T;
			if (token != null)
			{
				exprs = exprs.Rest;
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Checks that the member can be found in a given object.
		/// </summary>
		static void CheckIsMember (Member member, object obj)
		{
			if (!member.Info.DeclaringType.IsAssignableFrom (obj.GetType ()))
				Error (obj, string.Format ("Object of type {0} does have member {1}", 
					obj.GetType (), member));
		}
	}
}