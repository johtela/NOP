namespace NOP
{
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
			if (expr is Symbol)
				return new EvalResult (env, env.Lookup ((expr as Symbol).Name));
			if (expr is ExprList)
			{
				var list = expr as ExprList;
				var symbol = list.First as Symbol;
				if (symbol == null)
					Error (list.First, "Expected a function");
				
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
					default:
						return InvokeFunction (env, symbol, list.Rest);
				}
			}
			if (expr is Func)
				Error (expr, "Expected a symbol, list, or atom");
			return new EvalResult (env, expr);
		}

		/// <summary>
		/// Symbol definition evaluation.
		/// </summary>
		static internal EvalResult EvalDefine (Environment env, ExprList exprs)
		{
			var symbol = exprs.First as Symbol;
			if (symbol == null)
				Error (exprs.First, "Expected a symbol after define.");
			if (exprs.Rest.IsEmpty)
				Error (exprs.First, "Incomplete define clause.");
			var res = Eval (env, exprs.Rest.First).Result;
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
			EvalResult res;
			
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
					return env.Define (names.Rest.First, values)
					;
				}
				env = res.Env.Define (names.First, values.First);
				names = names.Rest;
				values = values.Rest;
			}
		}

		/// <summary>
		/// Create a function.
		/// </summary>
		private static EvalResult MakeFunction (Environment env, ExprList definition)
		{
			var list = definition.First as ExprList;
			if (list == null)
				Interpreter.Error (definition.First, "Expected list of parameters");
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
			var body = definition.Rest;
			if (body.IsEmpty)
				Interpreter.Error (definition.First, "Function body is missing");
			return new EvalResult (env, new Func (
					args => EvalBegin (BindParams (env, parameters, args), body).Result))
			;
		}

		/// <summary>
		/// Invoke a function.
		/// </summary>
		static internal EvalResult InvokeFunction (Environment env, Symbol fname, ExprList fargs)
		{
			var func = env.Lookup (fname.Name) as Func;
			if (func == null)
				Error (fname, "Expected a function");
			return new EvalResult (env, func (fargs.Map (expr => Eval (env, expr).Result)));
		}
	}
}