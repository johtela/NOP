namespace NOP.Testing
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Collections.Generic;
	using NOP.Collections;

	/// <summary>
	/// Helper class for building expressions.
	/// </summary>
	public abstract class ExprBuilder
	{
		#region Private case classes
		
		/// <summary>
		/// Build the expression.
		/// </summary>
		/// <returns>The expression defined by the builder.</returns>
		public abstract SExpr Build ();
		
		/// <summary>
		/// Builder for atom expressions.
		/// </summary>
		private class AtomBuilder<T> : ExprBuilder
		{
			private readonly T _value;
			
			public AtomBuilder (T value)
			{
				_value = value;
			}
			
			public override SExpr Build ()
			{
				return new SExpr.Literal (_value);
			}
		}
		
		/// <summary>
		/// Builder for symbol expressions.
		/// </summary>
		private class SymbolBuilder : ExprBuilder
		{
			private readonly string _name;
			
			public SymbolBuilder (string name)
			{
				_name = name;
			}
			
			public override SExpr Build ()
			{
				return new SExpr.Symbol (_name);
			}
		}
		
		/// <summary>
		/// Builder for list expressions.
		/// </summary>
		private class ListBuilder : ExprBuilder
		{
			private readonly StrictList<ExprBuilder> _items;
			
			public ListBuilder (IEnumerable<ExprBuilder> items)
			{
				_items = List.FromEnumerable (items);
			}
			
			public ListBuilder (StrictList<ExprBuilder> items)
			{
				_items = items;
			}

			public override SExpr Build ()
			{
				return new SExpr.List (Sequence.Create<SExpr> (_items.Map (eb => eb.Build ())));
			}
		}
		
		#endregion
		
		/// <summary>
		/// Create an atom expression builder.
		/// </summary>
		public static ExprBuilder A<T> (T value)
		{
			return new AtomBuilder<T> (value);
		}
		
		/// <summary>
		/// Create a symbol expression builder.
		/// </summary>
		public static ExprBuilder S (string name)
		{
			return new SymbolBuilder (name);
		}
		
		/// <summary>
		/// Create a list expression builder
		/// </summary>
		public static ExprBuilder L (params ExprBuilder[] items)
		{
			return new ListBuilder (items);
		}
		
		/// <summary>
		/// Create a list expression builder
		/// </summary>
		public static ExprBuilder L (StrictList<ExprBuilder> items)
		{
			return new ListBuilder (items);
		}

		public static ExprBuilder T (string typeName)
		{
			return S (typeName);
		}

		public static ExprBuilder T (string typeName, params ExprBuilder[] genericParams)
		{
			return L (S (typeName) | List.FromArray (genericParams));
		}

		public static ExprBuilder LT (ExprBuilder sourceType, ExprBuilder targetType)
		{
			return L (S ("->"), sourceType, targetType);
		}

		public static ExprBuilder V (string varName, ExprBuilder type)
		{
			return L (S (varName), type);
		}

		public static ExprBuilder Def (string defName, ExprBuilder type, ExprBuilder definition)
		{
			return L (S ("def"), V (defName, type), definition);
		}

		public static ExprBuilder Mod (string moduleName, params ExprBuilder[] definitions)
		{
			return L (S ("module") | (S (moduleName) | List.FromArray (definitions)));
		}

		/// <summary>
		/// Create a builder for the "quote" expression.
		/// </summary>
		public static ExprBuilder Quote (params ExprBuilder[] items)
		{
			return new ListBuilder (S ("quote") | List.Create (items));
		}
		
		/// <summary>
		/// Create a builder for the "if" expression,
		/// </summary>
		public static ExprBuilder If (ExprBuilder condition, ExprBuilder thenExpr, ExprBuilder elseExpr)
		{
			return new ListBuilder (List.Create (S ("if"), condition, thenExpr, elseExpr));
		}
		
		/// <summary>
		/// Create a builder for the "begin" expression.
		/// </summary>
		public static ExprBuilder Begin (params ExprBuilder[] items)
		{
			return new ListBuilder (S ("begin") | List.Create (items));
		}

		/// <summary>
		/// Single let binding.
		/// </summary>
		public static Tuple<string, ExprBuilder> Let (string name, ExprBuilder value)
		{
			return Tuple.Create (name, value);
		}

		/// <summary>
		/// Multiple let bindings
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static StrictList<Tuple<string, ExprBuilder>> And (params Tuple<string, ExprBuilder>[] lets)
		{
			return List.FromArray (lets);
		}

		/// <summary>
		/// Create a builder for the "let" expression.
		/// </summary>
		public static ExprBuilder LetIn (Tuple<string, ExprBuilder> let, ExprBuilder inExpr)
		{
			return new ListBuilder (List.Create (S ("let"), S (let.Item1), let.Item2, inExpr));
		}

		/// <summary>
		/// Create a builder for the "letrec" expression.
		/// </summary>
		public static ExprBuilder LetRec (StrictList<Tuple<string, ExprBuilder>> lets, ExprBuilder inExpr)
		{
			return new ListBuilder (BuildLetRec (lets, inExpr));
		}

		private static IEnumerable<ExprBuilder> BuildLetRec (StrictList<Tuple<string, ExprBuilder>> lets, 
			ExprBuilder inExpr)
		{
			yield return S ("letrec");
			yield return S (lets.First.Item1);
			yield return lets.First.Item2;
			for (var ands = lets.Rest; !ands.IsEmpty; ands = ands.Rest)
			{
				yield return S ("and");
				yield return S (ands.First.Item1);
				yield return ands.First.Item2;
			}
			yield return inExpr;
		}

		/// <summary>
		/// Create a list of parameter names.
		/// </summary>
		public static StrictList<string> P (params string[] args)
		{
			return List.Create (args);
		}
		
		/// <summary>
		/// Create a builder for the "lambda" expression.
		/// </summary>
		public static ExprBuilder Lambda (StrictList<string> args, ExprBuilder body)
		{
			return new ListBuilder (S ("lambda") | (L (args.Map (arg => S (arg))) | List.Cons (body)));
		}

		/// <summary>
		/// Create a builder for the function call expression.
		/// </summary>
		public static ExprBuilder Call (ExprBuilder function, params ExprBuilder[] args)
		{
			return new ListBuilder (function | List.Create (args));
		}
		
		/// <summary>
		/// Create a builder for the function call expression.
		/// </summary>
		public static ExprBuilder Call (string function, params ExprBuilder[] args)
		{
			return new ListBuilder (S (function) | List.Create (args));
		}
	}
	
	/// <summary>
	/// Helper class for inheriting the builder methods to the namespace.
	/// </summary>
	public class ExprUser : ExprBuilder
	{
		#region implemented abstract members of NOP.ExprBuilder
		
		public override SExpr Build ()
		{
			return null;
		}
		
		#endregion
	}
}