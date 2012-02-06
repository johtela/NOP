namespace NOP
{
	using System;
	using System.Collections.Generic;
	using NOP.Collections;

	/// <summary>
	/// Environment that contains the interpreter symbol bindings.
	/// </summary>
	public class Environment
	{
		private readonly Map<string, object> _map;

		/// <summary>
		/// Create an empty environment.
		/// </summary>
		public Environment ()
		{
			_map = Map<string, object>.Empty;
		}

		public Environment (IEnumerable<Type> types)
		{
			foreach (Type type in types)
			{
				if (type.IsClass)
				{
					
				}
			}
		}
		
		/// <summary>
		/// Create a new environment with given bindings.
		/// </summary>
		/// <param name="map">The map of bindings.</param>
		private Environment (Map<string, object> map)
		{
			_map = map;
		}

		/// <summary>
		/// Lookup for the symbol value.
		/// </summary>
		/// <param name="symbol">The symbol to be looked up.</param>
		/// <returns>The value of the symbol.</returns>
		public object Lookup (string symbol)
		{
			try
			{
				return _map [symbol];
			}
			catch (KeyNotFoundException)
			{
				throw new InterpreterException (null, string.Format ("Symbol {0} is not defined", symbol));
			}
		}

		/// <summary>
		/// Define a new symbol.
		/// </summary>
		/// <param name="symbol">The symbol to be defined.</param>
		/// <param name="value">The value of the symbol.</param>
		/// <returns>A new environment that contains the given symbol.</returns>
		public Environment Define (string symbol, object value)
		{
			return new Environment (_map.Add (symbol, value));
		}
	}
}

