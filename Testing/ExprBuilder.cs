namespace NOP
{
	using System;
	using System.Reflection;
	using SysColl = System.Collections.Generic;
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
			private readonly List<ExprBuilder> _items;
			
			public ListBuilder (SysColl.IEnumerable<ExprBuilder> items)
			{
				_items = List.Create (items);
			}
			
			public ListBuilder (List<ExprBuilder> items)
			{
				_items = items;
			}

			public override SExpr Build ()
			{
				return new SExpr.List (_items.Map (eb => eb.Build ()));
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
		public static ExprBuilder L (List<ExprBuilder> items)
		{
			return new ListBuilder (items);
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
		/// Create a builder for the "define" expression.
		/// </summary>
		public static ExprBuilder Define (string symbol, ExprBuilder value)
		{
			return new ListBuilder (List.Create (S ("define"), S (symbol), value));
		}
		
		/// <summary>
		/// Create a builder for the "let" expression.
		/// </summary>
		public static ExprBuilder Let (string symbol, ExprBuilder value, ExprBuilder inExpr)
		{
			return new ListBuilder (List.Create (S ("let"), S (symbol), value, inExpr));
		}
		
		/// <summary>
		/// Create a list of parameter names.
		/// </summary>
		public static List<string> P (params string[] args)
		{
			return List.Create (args);
		}
		
		/// <summary>
		/// Create a builder for the "lambda" expression.
		/// </summary>
		public static ExprBuilder Lambda (List<string> args, ExprBuilder body)
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