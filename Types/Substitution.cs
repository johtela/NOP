namespace NOP
{
	using System;
	using Collections;

	/// <summary>
	/// Substitution table provides a mapping from type variables to concrete monotypes.
	/// The substitution table is both the working data structure and the result of
	/// the infrence process.
	/// </summary>
	public class Substitution
	{
		private readonly Map<string, MonoType> _map;
			
		private Substitution (Map<string, MonoType> map)
		{
			_map = map;
		}
		
		/// <summary>
		/// Empty substitution table.
		/// </summary>
		public static Substitution Empty
		{
			get { return new Substitution (Map<string, MonoType>.Empty); }
		}
			
		/// <summary>
		/// Extend the substitution table with specified variable.
		/// </summary>
		public Substitution Extend (string name, MonoType type)
		{
			return new Substitution (_map.Add (name, type));
		}
			
		/// <summary>
		/// Lookup the type variable with the specified name. If the variable with
		/// the given name is not currently in the substitution table a new 
		/// variable is returned.
		/// </summary>
		public MonoType Lookup (string name)
		{
			return _map.Contains (name) ? _map [name] : new MonoType.Var (name);
		}
	}		
}